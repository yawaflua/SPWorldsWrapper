# SPWorldsWrapper
![](https://img.shields.io/badge/dotnet-.NET_7-green) 

Actions pass:


![](https://github.com/yawaflua/SPWorldsWrapper/actions/workflows/dotnet.yml/badge.svg)

Это довольно простой wrapper для работы с сайтом, а не с апи. Он позволяет:
- Находить более расширные данные о пользователь от сайта, нежели стандартные апи.
- Добавлять/удалять города на карте(ПРИ НАЛИЧИИ РОЛИ КАРТОГРАФА)
- Получать любые данные, используя HttpClient, внутри класса

# Установка
```cli 
dotnet add package SPWorldsWrapper
```

# Добавление функций
Если вы хотите помочь с разработкой, просьба создать Pull Request, где будет созданы нужные методы и, желательно, комментарии к ним.
Если вы хотите помочь с расширением этого wrapper`a, просьба создать Issue, где вы подробно изложите, что именно вам надо.

# Использование
## Перевод игроку, имея только никнейм
Ниже есть пример использования. В ходе этого примера мы переводим игроку 1 АР, получая карту по никнейму, а потом переводя на нее ары(через [библиотеку](https://github.com/Mih4n/spworlds-csharp-library)).
```cs
using SPWorldsWrapper;
using SPWorlds;

// инициилизация конструктора
// Токен надо получать на сайте, либо через cookies, либо через проверку request
SPWrapper wrapper = new SPWrapper ("AaBbCcDd:123123123");
// использование сторонней библиотеки от Mih4n
//! ТОКЕНЫ ДЛЯ SPWorlds и SPWrapper РАЗНЫЕ !
SPWorlds sp = new SPworlds("[ваш айди]","[ваш токен]");

// Пример, перевод на карту игрока по никнейму: 
SPUser user = await wrapper.getUserData("yawaflua");
sp.CreateTransaction(user.CardsOwned.First().number, 1, "Привет от yawaflua")
```
# FAQ
- Где получить токен для использования? - [тут](https://github.com/yawaflua/SPWorldsWrapper/blob/master/GETTOKEN.md)
- Как сделать так, чтобы токен не слетал? - Надо использовать впн, с выходной точкой в виде вашего сервера. Я использую [tailscale](https://tailscale.com/), но в России его больше нельзя скачать. Можно попробовать настроить OpenVPN или иные сервисы. [Тут](https://habr.com/ru/companies/ruvds/articles/726718/) например описано, как можно легко поднять с помощью сервиса RuVDS, где вы получите и VPN, и VDS. Я бы не советовал исполользовать RuVDS из-за большой стоимость и ужасного качества поддержки и серверов, но это выбор каждого.
