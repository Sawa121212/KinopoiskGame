import csv
import json
import os
import random
import requests
import sys
from datetime import datetime
from tqdm import tqdm
from concurrent.futures import ThreadPoolExecutor, as_completed
import threading

# Конфигурация
DB_FOLDER = "Random_movies_DB"
DB_FILE = os.path.join(DB_FOLDER, "searching_movies_DB.csv")
API_URL = "https://api.kinopoisk.dev/v1.4/movie"
MAX_WORKERS = 8
TARGET_NEW_MOVIES = 200
MAX_ATTEMPTS = TARGET_NEW_MOVIES * 5
REQUEST_TIMEOUT = 10
LOG_FILE = os.path.join(DB_FOLDER, "requests_log.txt")
MIN_ID = 1
MAX_ID = 9999999

# Блокировки
print_lock = threading.Lock()
file_lock = threading.Lock()
log_lock = threading.Lock()

# Глобальный флаг для остановки
stop_execution = False


def init_db():
    """Инициализирует базу данных и файл логов"""
    os.makedirs(DB_FOLDER, exist_ok=True)
    if not os.path.exists(DB_FILE):
        with open(DB_FILE, 'w', newline='', encoding='utf-8') as f:
            writer = csv.writer(f, delimiter=';')
            writer.writerow(['id', 'status'])

    with open(LOG_FILE, 'w', encoding='utf-8') as f:
        f.write("=== Request Log ===\n")


def load_existing_ids():
    """Загружает проверенные ID из файла"""
    existing_ids = set()
    if os.path.exists(DB_FILE):
        with open(DB_FILE, 'r', encoding='utf-8') as f:
            reader = csv.reader(f, delimiter=';')
            next(reader)
            for row in reader:
                if row and row[0].isdigit():
                    existing_ids.add(int(row[0]))
    return existing_ids


def save_result(movie_id: int, status: str):
    """Сохраняет результат в CSV файл"""
    with file_lock:
        with open(DB_FILE, 'a', newline='', encoding='utf-8') as f:
            writer = csv.writer(f, delimiter=';')
            writer.writerow([movie_id, status])


def log_request(movie_id: int, status: str, response=None):
    """Логирует детали запроса"""
    timestamp = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
    log_entry = f"{timestamp} | ID: {movie_id} | Status: {status}"

    if response:
        log_entry += f" | Code: {response.status_code}"
        if response.status_code == 200:
            log_entry += " (OK)"
        elif response.status_code in [400, 404]:
            log_entry += " (Not Found)"
        elif response.status_code == 403:
            log_entry += " (Daily Limit Exceeded)"
        else:
            log_entry += f" | Response: {response.text[:100]}..."

    with log_lock:
        with open(LOG_FILE, 'a', encoding='utf-8') as f:
            f.write(log_entry + "\n")

    if status != "ok":
        with print_lock:
            tqdm.write(log_entry)


def check_movie(api_key: str, movie_id: int):
    """Проверяет один фильм с логированием"""
    global stop_execution

    if stop_execution:
        return None

    headers = {"X-API-KEY": api_key}
    try:
        response = requests.get(
            f"{API_URL}/{movie_id}",
            headers=headers,
            timeout=REQUEST_TIMEOUT
        )

        if response.status_code == 200:
            data = response.json()
            if data.get('id'):
                log_request(movie_id, "ok", response)
                save_result(movie_id, 'ok')
                return {
                    'id': data['id'],
                    'name': data.get('name'),
                    'alternativeName': data.get('alternativeName'),
                    'year': data.get('year'),
                    'rating': {
                        'kp': data.get('rating', {}).get('kp')
                    },
                    'poster': {
                        'url': data.get('poster', {}).get('url')
                    }
                }
            else:
                log_request(movie_id, "invalid_data", response)

        elif response.status_code in [400, 404]:
            log_request(movie_id, "not_found", response)
            save_result(movie_id, '404')
        elif response.status_code == 403:
            log_request(movie_id, "daily_limit", response)
            stop_execution = True  # Устанавливаем флаг остановки
            return None
        else:
            log_request(movie_id, f"error_{response.status_code}", response)

    except requests.exceptions.Timeout:
        log_request(movie_id, "timeout")
    except requests.exceptions.ConnectionError:
        log_request(movie_id, "connection_error")
    except Exception as e:
        log_request(movie_id, f"exception: {str(e)}")

    return None


def worker_task(api_key, movie_id, progress):
    """Задача для отдельного потока"""
    result = check_movie(api_key, movie_id)
    if result and result.get('id'):
        progress.update(1)
        return result
    return None


def print_summary(found_count: int, total_attempts: int, limit_exceeded: bool = False):
    """Выводит статистику по завершении"""
    summary = (
        f"\n=== Summary ===\n"
        f"Total attempts: {total_attempts}\n"
        f"Found movies: {found_count}\n"
        f"Success rate: {found_count / max(1, total_attempts):.1%}\n"
    )

    if limit_exceeded:
        summary += "WARNING: Daily API limit exceeded!\n"

    summary += f"Log file: {LOG_FILE}\n"

    with print_lock:
        print(summary)


def find_random_movies(api_key: str, existing_ids: set):
    """Основная функция поиска"""
    global stop_execution
    found_movies = []
    attempts = 0
    limit_exceeded = False

    with tqdm(total=TARGET_NEW_MOVIES, desc="Finding movies") as progress:
        with ThreadPoolExecutor(max_workers=MAX_WORKERS) as executor:
            while (len(found_movies) < TARGET_NEW_MOVIES and
                   attempts < MAX_ATTEMPTS and
                   not stop_execution):

                batch_size = min(MAX_WORKERS * 2, TARGET_NEW_MOVIES - len(found_movies))
                batch_ids = []

                while (len(batch_ids) < batch_size and
                       attempts < MAX_ATTEMPTS and
                       not stop_execution):

                    movie_id = random.randint(MIN_ID, MAX_ID)
                    if movie_id not in existing_ids:
                        batch_ids.append(movie_id)
                        existing_ids.add(movie_id)
                        attempts += 1

                futures = [
                    executor.submit(worker_task, api_key, mid, progress)
                    for mid in batch_ids
                ]

                for future in as_completed(futures):
                    if stop_execution:
                        limit_exceeded = True
                        break

                    result = future.result()
                    if result:
                        found_movies.append(result)

                if stop_execution:
                    break

    print_summary(len(found_movies), attempts, limit_exceeded)
    return found_movies


def save_results(found_movies: list):
    """Сохраняет результаты в файл"""
    if not found_movies:
        return None

    timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
    output_file = os.path.join(DB_FOLDER, f"found_{timestamp}.json")

    with open(output_file, 'w', encoding='utf-8') as f:
        json.dump(found_movies, f, ensure_ascii=False, indent=2)

    return output_file


if __name__ == "__main__":
    try:
        from DevKey import KINOPOISK_API_KEY
    except ImportError:
        print("Error: Create DevKey.py with KINOPOISK_API_KEY")
        sys.exit(1)

    init_db()
    existing_ids = load_existing_ids()

    print(f"Database: {len(existing_ids)} IDs | Target: {TARGET_NEW_MOVIES} new")
    print(f"ID range: {MIN_ID}-{MAX_ID}")
    print(f"Logging to: {LOG_FILE}\n")

    try:
        found = find_random_movies(KINOPOISK_API_KEY, existing_ids)

        if found:
            output_file = save_results(found)
            print(f"Results saved to: {output_file}")
            print(f"Database updated: {DB_FILE}")
        else:
            print("\nNo new movies found.")

    except KeyboardInterrupt:
        print("\nExecution interrupted by user")
        sys.exit(0)
    except Exception as e:
        print(f"\nCritical error: {str(e)}")
        sys.exit(1)