using Newtonsoft.Json;
using SPWorldsWrapper.Types;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace SPWorldsWrapper
{
    public class AuthentificationError : Exception
    {
        public AuthentificationError(string? message, Exception? innerException) : base(message, innerException) { }
        public AuthentificationError() : base() { }
        public AuthentificationError(string? message) : base(message) { }
    }
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
            var spwLoginMethod = spwLogin();
            if (!spwLoginMethod.Result)
            {
                throw new AuthentificationError("Ошибка парсинга данных от сайта. Проверьте токен, IP сервера и статус сайта SPWORLDS.RU");
            }
            
        }
        /// <summary>
        /// Авторизация на сайте. Постоянное обновление не требуется, но желательно.
        /// Следите за выводом в консоль, там должны быть ваши данные о всех серверах. 
        /// Если иное - токен слетел!
        /// </summary>
        /// <returns>Ничего</returns>
        public async Task<bool> spwLogin()
        {
            var content = new StringContent(@"{}");
            var responseMessage = await client.PostAsync("auth/refresh_token", content);
            var bodyFromSPW = await responseMessage.Content.ReadAsStringAsync();
            var serializedBody = JsonNode.Parse(bodyFromSPW);
            if (serializedBody == null)
            {
                Console.Error.WriteLine("error: Some error returned from site.");
                Console.WriteLine("debug: please, check your authorization token");
            }
            else
            {
                Console.WriteLine(await responseMessage.Content.ReadAsStringAsync());
            }
           
            
            return serializedBody != null;
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
        /// <returns><see cref="SPUser" /> пользователь от сайта, или null.</returns>
        public async Task<SPUser?> getUserData(string userName)
        {
            var request = await client.GetAsync($"pl/accounts/{userName}");
            SPUser? response = JsonConvert.DeserializeObject<SPUser>(request.Content.ReadAsStringAsync().Result.ToString());
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
