import requests
import time
import json
import os
from datetime import datetime
from tqdm import tqdm
from typing import List, Dict, Optional

try:
    from DevKey import KINOPOISK_API_KEY
except ImportError:
    print("Ошибка: Создайте файл DevKey.py с переменной KINOPOISK_API_KEY")
    exit(1)
except AttributeError:
    print("Ошибка: В файле DevKey.py отсутствует переменная KINOPOISK_API_KEY")
    exit(1)


class KinopoiskAPI:
    BASE_URL = "https://api.kinopoisk.dev/v1.4/"

    def __init__(self, api_key: str):
        self.api_key = api_key
        self.headers = {"X-API-KEY": api_key}

    def get_movies_by_category(self, category: str, limit: int = 50, page: int = 1) -> Optional[Dict]:
        """Получить фильмы по категории"""
        endpoints = {
            "top250": "movie?page={page}&limit={limit}&lists=top250",
            "top500": "movie?page={page}&limit={limit}&lists=top500",
            "popular-films": "movie?page={page}&limit={limit}&type=movie&notNullFields=rating.kp&sortField=votes.kp&sortType=-1"
        }

        if category not in endpoints:
            raise ValueError(f"Unknown category: {category}")

        url = self.BASE_URL + endpoints[category].format(page=page, limit=limit)

        try:
            response = requests.get(url, headers=self.headers)
            response.raise_for_status()
            return response.json()
        except requests.exceptions.RequestException as e:
            print(f"\nError fetching {category} (page {page}): {str(e)}")
            if hasattr(response, 'text'):
                print(f"Response details: {response.text}")
            return None

    def get_all_movies_from_category(self, category: str, max_movies: int, progress_bar: tqdm) -> List[Dict]:
        """Получить все фильмы из категории (с пагинацией)"""
        movies = []
        limit = 50  # Оптимальное количество фильмов за один запрос
        pages = (max_movies + limit - 1) // limit

        for page in range(1, pages + 1):
            current_limit = min(limit, max_movies - len(movies))
            if current_limit <= 0:
                break

            data = self.get_movies_by_category(category, limit=current_limit, page=page)
            if data and 'docs' in data:
                movies.extend(data['docs'])
                progress_bar.update(len(data['docs']))

            # Задержка для соблюдения лимитов API
            time.sleep(0.5)

        return movies


def create_output_folder() -> str:
    """Создать папку для результатов с текущей датой и временем"""
    now = datetime.now()
    folder_name = f"Build_{now.strftime('%Y-%m-%d_%H-%M-%S')}"
    os.makedirs(folder_name, exist_ok=True)
    return folder_name


def save_to_file(movies: List[Dict], folder: str, category: str):
    """Сохранить данные о фильмах в файл с датой в имени"""
    now = datetime.now()
    filename = f"{folder}/{category}_movies_{now.strftime('%Y-%m-%d')}.json"

    with open(filename, 'w', encoding='utf-8') as f:
        json.dump(movies, f, ensure_ascii=False, indent=2)

    print(f"\nSaved {len(movies)} movies to {filename}")


def main():
    output_folder = create_output_folder()
    print(f"Created output folder: {output_folder}")

    kp = KinopoiskAPI(KINOPOISK_API_KEY)

    categories = {
        #"top250": 250,
        #"top500": 500,
        "popular-films": 1000
    }

    for category, max_movies in categories.items():
        print(f"\nFetching movies from category: {category.upper()}")

        with tqdm(total=max_movies, desc=f"Progress {category}", unit="movie") as pbar:
            movies = kp.get_all_movies_from_category(category, max_movies, pbar)

        save_to_file(movies, output_folder, category)

    print("\nDone! All data has been collected.")


if __name__ == "__main__":
    main()