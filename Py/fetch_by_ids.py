import requests
import json
from datetime import datetime
import os
from tqdm import tqdm
from concurrent.futures import ThreadPoolExecutor, as_completed
import threading

# Глобальная блокировка для безопасного доступа к общим ресурсам
print_lock = threading.Lock()
data_lock = threading.Lock()


def fetch_single_movie(api_key: str, movie_id: int):
    """Запрашивает данные одного фильма по ID"""
    url = f"https://api.kinopoisk.dev/v1.4/movie/{movie_id}"
    headers = {
        "accept": "application/json",
        "X-API-KEY": api_key
    }

    try:
        with requests.get(url, headers=headers) as response:
            response.raise_for_status()
            movie_data = response.json()

            if not movie_data.get("id"):
                return None

            return {
                "id": movie_data.get("id"),
                "name": movie_data.get("name"),
                "alternativeName": movie_data.get("alternativeName"),
                "year": movie_data.get("year"),
                "rating": {
                    "kp": movie_data.get("rating", {}).get("kp")
                },
                "poster": {
                    "url": movie_data.get("poster", {}).get("url")
                }
            }
    except Exception as e:
        with print_lock:
            tqdm.write(f"Error fetching movie ID {movie_id}: {str(e)}")
        return None


def fetch_movies_parallel(api_key: str, id_range: range, max_workers: int = 10):
    """Параллельный сбор фильмов по диапазону ID"""
    movies = []

    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        # Создаем futures для каждого ID
        futures = {
            executor.submit(fetch_single_movie, api_key, movie_id): movie_id
            for movie_id in id_range
        }

        # Прогресс-бар для отслеживания выполнения
        with tqdm(total=len(id_range), desc="Fetching movies") as pbar:
            for future in as_completed(futures):
                result = future.result()
                if result:
                    with data_lock:
                        movies.append(result)
                pbar.update(1)

    return movies


def main():
    try:
        from DevKey import KINOPOISK_API_KEY
    except ImportError:
        print("Error: Create DevKey.py with KINOPOISK_API_KEY variable")
        return

    print("Enter the range of movie IDs to fetch")
    try:
        start_id = int(input("Start ID: "))
        end_id = int(input("End ID: "))

        if start_id > end_id:
            print("Error: Start ID must be less than or equal to End ID")
            return
    except ValueError:
        print("Error: Please enter valid numbers")
        return

    # Оптимальное количество потоков (можно регулировать)
    max_workers = 15
    print(f"\nFetching movies from {start_id} to {end_id} using {max_workers} threads...")

    movies = fetch_movies_parallel(
        KINOPOISK_API_KEY,
        range(start_id, end_id + 1),
        max_workers=max_workers
    )

    output_folder = f"Build_{datetime.now().strftime('%Y-%m-%d_%H-%M-%S')}"
    os.makedirs(output_folder, exist_ok=True)
    filename = f"{output_folder}/movies_ids_{start_id}-{end_id}.json"

    with open(filename, 'w', encoding='utf-8') as f:
        json.dump(movies, f, ensure_ascii=False, indent=2)

    print(f"\nSuccess! Saved {len(movies)} movies to {filename}")


if __name__ == "__main__":
    main()