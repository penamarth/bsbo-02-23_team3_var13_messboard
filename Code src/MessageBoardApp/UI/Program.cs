using MessageBoardApp.Application;
using MessageBoardApp.Application.Interfaces;
using MessageBoardApp.Domain.Entities;
using MessageBoardApp.Infrastructure.ExternalAPIs;
using MessageBoardApp.Infrastructure.Repositories;
using MessageBoardApp.Infrastructure.Services;
using MessageBoardApp.Infrastructure.Adapters;
using System;

namespace MessageBoardApp.Presentation
{
    class UI
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var database = new Database();
            IUsersRepository usersRepository = new UsersRepository(database);
            IAdvertisementsRepository advertisementsRepository = new AdvertisementsRepository(database);
            var orderServiceAPI = new OrderServiceAPI();
            IDeliveryServiceAdapter deliveryServiceAdapter = new DeliveryServiceAdapter(orderServiceAPI);
            var searchEngine = new SearchEngine(database);
            var geoServiceAPI = new GeoServiceAPI();

            var facade = new MessageBoardFacade(
                usersRepository,
                advertisementsRepository,
                deliveryServiceAdapter,
                searchEngine
            );

            Console.WriteLine("=== ДЕМОНСТРАЦИЯ РЕГИСТРАЦИИ ПОЛЬЗОВАТЕЛЯ ===\n");

            Console.WriteLine("== Этап 1: Пользователь нажимает 'Зарегистрироваться' ==");
            Console.WriteLine("UI: Показывает форму регистрации\n");

            Console.WriteLine("== Этап 2: Пользователь заполняет основные данные ==");
            string userName = "Иван Петров";
            string userEmail = "ivan.petrov@example.com";
            string userPassword = "SecurePassword123";
            Console.WriteLine($"UI: Заполнены данные - Имя: {userName}, Email: {userEmail}, Пароль: ***\n");

            Console.WriteLine("== Этап 3: Запрос разрешения на геолокацию ==");
            Console.WriteLine("UI: Вызов requestPermission()");
            geoServiceAPI.RequestPermission();
            Console.WriteLine("GeoServiceAPI: Запрос разрешения отправлен");
            Console.WriteLine("Пользователь: Предоставляет разрешение\n");

            Console.WriteLine("== Этап 4: Определение локации ==");
            Console.WriteLine("UI: Вызов detectLocation()");
            string detectedLocation = geoServiceAPI.DetectLocation();
            Console.WriteLine($"GeoServiceAPI: Локация определена - {detectedLocation}");
            Console.WriteLine($"UI: Автозаполнение поля локации значением '{detectedLocation}'\n");

            Console.WriteLine("== Этап 5: Корректировка адреса ==");
            string finalLocation = "Москва, ул. Ленина, д. 10";
            Console.WriteLine($"Пользователь: Корректирует адрес на '{finalLocation}'\n");

            Console.WriteLine("== Этап 6: Отправка формы регистрации ==");
            Console.WriteLine("Пользователь: Нажимает 'Зарегистрироваться'\n");

            var registrationData = new List<string> { userName, userEmail, userPassword, finalLocation };
            var newUser = facade.CreateUser(registrationData);

            if (newUser != null)
            {
                Console.WriteLine($"\nUI: Редирект в личный кабинет");
                Console.WriteLine($"UI: Показывает сообщение об успехе - 'Регистрация успешна!'");
                Console.WriteLine($"UI: Загружен профиль пользователя:");
                Console.WriteLine($"  - ID: {newUser.UserId}");
                Console.WriteLine($"  - Имя: {newUser.Name}");
                Console.WriteLine($"  - Email: {newUser.Email}");
                Console.WriteLine($"  - Локация: {newUser.Location}");
            }
            else
            {
                Console.WriteLine($"\nUI: Показывает ошибку регистрации");
            }

            Console.WriteLine("\n\n=== ДЕМОНСТРАЦИЯ УПРАВЛЕНИЯ ОБЪЯВЛЕНИЯМИ ===\n");

            var seller = newUser;
            uint sellerId = seller.UserId;

            Console.WriteLine("== Этап 1: Создание первого объявления ==");
            Console.WriteLine("Продавец: Нажимает 'Добавить объявление'");
            var ad1Values = new List<string> { "Смартфон Samsung Galaxy", "Смартфон в хорошем состоянии", "35000" };
            var advertisement1 = facade.CreateAdvertisment(sellerId, ad1Values);
            Console.WriteLine();

            Console.WriteLine("== Этап 2: Создание второго объявления ==");
            Console.WriteLine("Продавец: Нажимает 'Добавить объявление'");
            var ad2Values = new List<string> { "Планшет iPad", "Планшет с чехлом и защитной пленкой", "42000" };
            facade.CreateAdvertisment(sellerId, ad2Values);
            Console.WriteLine();

            Console.WriteLine("== Этап 3: Просмотр своих объявлений ==");
            Console.WriteLine("Продавец: Нажимает 'Мои объявления'");
            var userAdvertisements = facade.GetUserAdvertisments(sellerId);
            Console.WriteLine($"UI: Отображение списка объявлений продавца ({userAdvertisements.Count} шт.):");
            foreach (var ad in userAdvertisements)
            {
                Console.WriteLine($"  - {ad.Name} ({ad.Cost}₽)");
            }
            Console.WriteLine();

            Console.WriteLine("\n\n=== ДЕМОНСТРАЦИЯ ОФОРМЛЕНИЯ ЗАКАЗА ===\n");
            
            var buyer = facade.CreateUser(new List<string>
            {
                "Петр Иванов", "buyer@example.com", "SecurePassword123", "Москва, ул. Ленина, д. 11" 
            });

            Console.WriteLine("== Этап 1: Создание заказа ==");
            Console.WriteLine("UI: Покупатель нажимает 'Оформить заказ'");
            var order = facade.CreateOrder(buyer.UserId, advertisement1.Id);
            Console.WriteLine();

            Console.WriteLine("== Этап 2: Выбор способа доставки ==");
            Console.WriteLine("UI: Покупатель выбирает 'Доставка через сторонний сервис'");
            uint selectedServiceId = 1;
            facade.SelectDelivery(buyer.UserId, order.Id, selectedServiceId);
            Console.WriteLine();

            Console.WriteLine("== Этап 3: Расчет стоимости доставки ==");
            Console.WriteLine("UI: Отправка запроса на расчет стоимости");
            var deliveryQuote = facade.RequestDeliveryQuote(buyer.UserId, order.Id, selectedServiceId);
            Console.WriteLine($"UI: Отображение результата - Стоимость: {deliveryQuote.Cost}₽, Статус: {deliveryQuote.Status}");
            Console.WriteLine();

            Console.WriteLine("== Этап 4: Оплата заказа ==");
            Console.WriteLine("UI: Покупатель подтверждает и переходит к оплате");
            var paymentResult = facade.ProcessPayment(buyer.UserId, order.Id);
            Console.WriteLine($"UI: Результат платежа - {paymentResult.Name}, Статус: {paymentResult.Status}");
            Console.WriteLine();

            Console.WriteLine("== Этап 5: Генерация чека ==");
            var receipt = facade.GenerateReceipt(buyer.UserId, order.Id);
            Console.WriteLine($"UI: Отображение чека");
            Console.WriteLine($"  - Товаров: {receipt.TotalItems}");
            Console.WriteLine($"  - Сумма товара: {order.TotalSum}₽");
            Console.WriteLine($"  - Стоимость доставки: {receipt.DeliveryCost}₽");
            Console.WriteLine($"  - Итого: {receipt.TotalSum}₽");
            Console.WriteLine();

            Console.WriteLine("\n\n=== ДЕМОНСТРАЦИЯ УПРАВЛЕНИЯ ИЗБРАННЫМ ===\n");

            buyer = newUser;
            uint buyerId = buyer.UserId;

            Console.WriteLine("== Этап 1: Поиск объявлений ==");
            Console.WriteLine("Покупатель: Открывает каталог и ищет 'Смартфон'");
            string searchQuery = "Смартфон";
            var searchResults = facade.Search(searchQuery);
            Console.WriteLine($"UI: Найдено {searchResults.Count} объявлений\n");

            foreach (var ad in searchResults)
            {
                Console.WriteLine($"  - {ad.Name}: {ad.Cost}₽ (ID: {ad.Id})");
            }
            Console.WriteLine();

            Console.WriteLine("== Этап 2: Добавление объявлений в избранное ==");
            if (searchResults.Count > 0)
            {
                Console.WriteLine("Покупатель: Нажимает 'Добавить в избранное' для первого объявления");
                facade.AddToFavorites(buyerId, searchResults[0].Id);
                Console.WriteLine("UI: Подтверждение добавления\n");
            }

            if (searchResults.Count > 1)
            {
                Console.WriteLine("Покупатель: Нажимает 'Добавить в избранное' для второго объявления");
                facade.AddToFavorites(buyerId, searchResults[1].Id);
                Console.WriteLine("UI: Подтверждение добавления\n");
            }

            Console.WriteLine("== Этап 3: Переход в раздел 'Избранное' ==");
            Console.WriteLine("Покупатель: Переходит в раздел 'Избранное'");
            var favouriteAdvertisements = facade.GetFavorites(buyerId);
            Console.WriteLine($"UI: Отображение списка избранного ({favouriteAdvertisements.Count} шт.):");
            foreach (var ad in favouriteAdvertisements)
            {
                Console.WriteLine($"  - {ad.Name} ({ad.Cost}₽)");
            }
            Console.WriteLine();

            Console.WriteLine("== Этап 4: Удаление из избранного ==");
            if (favouriteAdvertisements.Count > 0)
            {
                Console.WriteLine($"Покупатель: Нажимает 'Удалить из избранного' для '{favouriteAdvertisements[0].Name}'");
                facade.RemoveFromFavorites(buyerId, favouriteAdvertisements[0].Id);
                Console.WriteLine("UI: Подтверждение удаления\n");

                Console.WriteLine("Покупатель: Проверяет обновленный список избранного");
                var updatedFavourites = facade.GetFavorites(buyerId);
                Console.WriteLine($"UI: Отображение обновленного списка ({updatedFavourites.Count} шт.):");
                foreach (var ad in updatedFavourites)
                {
                    Console.WriteLine($"  - {ad.Name} ({ad.Cost}₽)");
                }
            }
            Console.WriteLine();

        }
    }
}
