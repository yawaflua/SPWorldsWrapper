using Newtonsoft.Json;
using SPWorldsWrapper.Types;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace SPWorldsWrapper
{
    public class SPWrapper
    {
        public readonly HttpClient client;

        /// <summary>
        /// Асинхронный wrapper для работы напрямую с сайтом, а не с API SPWorlds.ru
        /// </summary>
        /// <param name="token">Токен от сайта(смотреть README.md)</param>
        public SPWrapper(string token) 
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            client = new(handler);
            client.BaseAddress = new Uri("https://spworlds.ru/api/");
            cookieContainer.Add(client.BaseAddress, new Cookie("jeff", token));
            spwLogin();
            
        }
        /// <summary>
        /// Авторизация на сайте. Постоянное обновление не требуется, но желательно.
        /// Следите за выводом в консоль, там должны быть ваши данные о всех серверах. 
        /// Если иное - токен слетел!
        /// </summary>
        /// <returns>Ничего</returns>
        public async Task spwLogin()
        {
            var content = new StringContent(@"{}");
            var responseMessage = await client.PostAsync("auth/refresh_token", content);
            await Console.Out.WriteLineAsync(await responseMessage.Content.ReadAsStringAsync());
        }
        /// <summary>
        /// Получение данных пользователя по юзернейму.
        /// Рекомендованное использование:
        /// <code>
        /// SPWrapper Wrapper = new ("aA11Bb");
        /// SPUser user;
        /// try 
        /// {
        ///     user = await spwrapper.getUserData("yawaflua");
        /// }
        /// catch (Exception ex)
        /// {
        ///     // Ваша логика
        /// }
        /// </code>
        /// </summary>
        /// <param name="userName">Никнейм пользователя(из майнкрафта)</param>
        /// <example> var a = "a";</example>
        /// <returns><see cref="SPUser" /> пользователь от сайта, или ошибку.</returns>
        public async Task<SPUser> getUserData(string userName)
        {
            var request = await client.GetAsync($"pl/accounts/{userName}");
            await Console.Out.WriteLineAsync(request.Content.ReadAsStringAsync().Result);
            SPUser response = JsonConvert.DeserializeObject<SPUser>(request.Content.ReadAsStringAsync().Result.ToString());
            return response;
        }
        /// <summary>
        /// Получение всех городов на сайте.
        /// </summary>
        /// <returns>Список из <see cref="SPCity"/>, содержащий все города на сервере</returns>
        public async Task<IEnumerable<SPCity>> getAllSities()
        {
            var citiesArray = new List<SPCity>();
            var request = await client.GetAsync("https://spworlds.ru/api/pl/cities");
            JsonNode jsonBody = await request.Content.ReadFromJsonAsync<JsonNode>();
            foreach (JsonNode node in jsonBody.AsArray())
            {
                citiesArray.Add(JsonConvert.DeserializeObject<SPCity>(node.ToJsonString()));
            }
            return citiesArray;
        }

        /// <summary>
        /// Добавление города на карту(прим. у вас должна быть роль Картограф на карте, иначе будет ошибка)
        /// </summary>
        /// <param name="city">Город, который вы хотите добавить</param>
        /// <returns><see cref="SPCity"/> или null, если нет роли картографа.</returns>
        public async Task<SPCity?> addSityOnMap(SPCity city)
        {
            try
            {
                var request = await client.PostAsync("https://spworlds.ru/api/pl/cities", JsonContent.Create(city));
                if (request.StatusCode.HasFlag(HttpStatusCode.OK))
                {
                    return city;
                }
                else
                {
                    throw new Exception($"Unknown error from site! {request.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// Удаление города с карты(прим. у вас должна быть роль Картограф на карте, иначе будет ошибка)
        /// </summary>
        /// <param name="city">Город, который вы хотите удалить</param>
        /// <returns><see cref="SPCity"/>: удаленный город или <see cref="null"/> если нет роли игрок</returns>
        public async Task<SPCity?> deleteSityFromMap(SPCity city)
        {
            var request = await client.DeleteAsync($"https://spworlds.ru/api/pl/cities/{city.id}");
            if (request.StatusCode.Equals(200))
            {
                return city;
            }
            else
            {
                return null;
            }

        }
    }
}
