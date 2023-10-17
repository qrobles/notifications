using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Data;
using System.IO;
using System;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Formats.Asn1;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using Telegram.Bot.Types.InlineQueryResults;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Xml.Linq;
using Microsoft.VisualBasic;

var botClient = new TelegramBotClient("6552814503:AAGXwH3M-EWOgGVNIOn5tTo_8DS-ehpB8Fw");

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};
botClient.StartReceiving(
    updateHandler: HandleUpdatesAsync,
    pollingErrorHandler: HandleErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

Timer _timer = null;

Console.ReadLine();

void TimerCallback(Object o)
{
    Console.WriteLine("In TimerCallback: " + DateTime.Now);
}

var me = await botClient.GetMeAsync();

// Send cancellation request to stop bot
//cts.Cancel();
static string Rearm(string name)
{
    name = name.Replace("-", "\\-").Replace("+", "\\+").Replace(".", "\\.").Replace("(", "\\(").Replace(")", "\\)").Replace("/", "\\.");
    return name;
}
async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    ReplyKeyboardRemove hide = new ReplyKeyboardRemove();
    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await HandleMessage(botClient, update.Message);
        return;
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }
    if (update.Message != null)
    {
        if (update.Message.Type == MessageType.Contact)
        {
            await GetContactAsync(botClient, MessageType.Contact, update.Message);
            return;
        }
        if (update.Message.Type == MessageType.Location)
        {
            await GetLocationAsync(botClient, MessageType.Contact, update.Message);
            return;
        }
    }
}
async Task HandleMessage(ITelegramBotClient botClient, Message message) //Сообщения
{
    var username = message.Chat.Username;
    var name = message.Chat.FirstName + " " + message.Chat.LastName;
    var telegramId = message.From.Id;
    var chatId = message.Chat.Id;
    var ss = message.Text;

    InlineKeyboardMarkup start = new(new[]
    {
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Старт", callbackData: "inlStart"),
        },
    });

    if (ss == "/start")
    {
        await botClient.SendTextMessageAsync(chatId, Rearm("Доброго времени суток\\!\nНажмите на кнопку _*Старт*_ чтобы начать тестирование"), replyMarkup: start, parseMode: ParseMode.MarkdownV2);
    }
}
async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery) //Кнопки
{
    ReplyKeyboardRemove hide = new ReplyKeyboardRemove();
    var chatId = callbackQuery.Message.Chat.Id;

    ReplyKeyboardMarkup shareContact = new(new[] { KeyboardButton.WithRequestContact("➡Отправить контакт⬅") });
    ReplyKeyboardMarkup shareContact2 = new(new[] { KeyboardButton.WithRequestLocation("➡Отправить местоположение⬅") });

    InlineKeyboardButton link = new InlineKeyboardButton("");
    link.Text = "Я ссылочка";
    link.Url = "https://vk.com/flame_chanel";
    InlineKeyboardButton linkBack = new InlineKeyboardButton("");
    linkBack.Text = "Я кнопочка назад";
    linkBack.CallbackData = "linkBack";

    InlineKeyboardMarkup inlineKeyboard = new(new[]
    {
        link, 
        linkBack,
    });
    InlineKeyboardMarkup levels = new(new[]
    {
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Курсы", callbackData: "inCourses")
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Запросить данные", callbackData: "inlEasy"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Запросить файлы", callbackData: "inlHard"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Нажать на ссылочку", callbackData: "inlLink"),
        },
         
    });
    InlineKeyboardMarkup courses = new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(text: "Бизнес и управление", callbackData: "busines"),
        }
    });
    InlineKeyboardMarkup levels2 = new(new[]
    {
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Отправить паспорт Telegram", callbackData: "inlPasport"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Отправить местоположение", callbackData: "inlLocation"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "inlBack2"),
        },
    });
    InlineKeyboardMarkup files = new(new[]
    {
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Отправить фото", callbackData: "inlPhoto"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Отправить видео", callbackData: "inlVideo"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Отправить музыку", callbackData: "inlMusic"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Отправить контакт", callbackData: "inlContact"),
        },
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Назад", callbackData: "inlBack"),
        },
    });
 
    try 
    {
        if (callbackQuery.Data == "inCourses")
        {
            await botClient.EditMessageCaptionAsync(chatId, callbackQuery.Message.MessageId, "Выберите интересующий вас курс", replyMarkup: courses);
        }
        if (callbackQuery.Data == "inlLink")
        {
            await botClient.EditMessageTextAsync(chatId, callbackQuery.Message.MessageId, "Нажми на ссылочку!", replyMarkup: inlineKeyboard);
        }
        if (callbackQuery.Data == "inlStart")
        {
            await botClient.EditMessageTextAsync(chatId, callbackQuery.Message.MessageId, "Выберите категорию:", replyMarkup: levels);
        }
        if (callbackQuery.Data == "inlEasy")
        {
            await botClient.EditMessageTextAsync(chatId, callbackQuery.Message.MessageId, "Выберите что хотите отправить:", replyMarkup: levels2);
        }
        if (callbackQuery.Data == "inlPasport")
        {
            await botClient.SendTextMessageAsync(chatId, "Нажмите на кнопку для отправки паспорта Telegram", replyMarkup: shareContact);
        }
        if (callbackQuery.Data == "inlLocation")
        {
            await botClient.SendTextMessageAsync(chatId, "Нажмите на кнопку для отправки местоположения", replyMarkup: shareContact2);
        }
        if (callbackQuery.Data == "inlHard")
        {
            await botClient.EditMessageTextAsync(chatId, callbackQuery.Message.MessageId, "Выберите тип файла\nВнимание! Тип видео может загружаться долго.", replyMarkup: files);
        }
        if (callbackQuery.Data == "inlPhoto")
        {
            await botClient.SendPhotoAsync(callbackQuery.Message.Chat.Id, "https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg");
        }
        if (callbackQuery.Data == "inlBack")
        {
            await botClient.EditMessageTextAsync(chatId, callbackQuery.Message.MessageId, "Выберите категорию:", replyMarkup: levels);
        }
        if (callbackQuery.Data == "inlBack2")
        {
            await botClient.EditMessageTextAsync(chatId, callbackQuery.Message.MessageId, "Выберите категорию:", replyMarkup: levels);
        }
        if (callbackQuery.Data == "linkBack")
        {
            await botClient.EditMessageTextAsync(chatId, callbackQuery.Message.MessageId, "Выберите категорию:", replyMarkup: levels);
        }
        if (callbackQuery.Data == "inlVideo")
        {
            await using var stream = System.IO.File.OpenRead("C:\\Users\\Delfa\\Videos\\test\\rick_video.mp4");
            await botClient.SendVideoAsync(
            chatId: chatId,
            video: stream,
            thumb: "https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg",
            supportsStreaming: true
            );
        }
        if (callbackQuery.Data == "inlMusic")
        {
            await botClient.SendAudioAsync(
            chatId: chatId,
            audio: "https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3",
            performer: "Имя автора (любое)",
            title: "Название песни (любое)",
            duration: 100// in seconds
            );
        }
        if (callbackQuery.Data == "inlContact")
        {
            await botClient.SendContactAsync(
            chatId: chatId,
            phoneNumber: "+1234567890",
            firstName: "Иван",
            vCard: "BEGIN:VCARD\n" +
                   "VERSION:3.0\n" +
                   "N:Иван;Иванов\n" +
                   "ORG:Этажи\n" +
                   "TEL;TYPE=voice,work,pref:+1234567890\n" +
                   "EMAIL:Iban@mfalcon.com\n" +
                   "END:VCARD");
        }
    }
    catch (Exception ex)
    {
        FileStream notificationsStream = new FileStream("D:\\Desktop\\krasota-Vanya\\ErrorLogs.TXT", FileMode.Append); // Логи

        string sMessage = ex.ToString() + "\n\n" + DateTime.Now;
        StreamWriter streamWriter = new StreamWriter(notificationsStream);

        streamWriter.WriteLine(sMessage);

        streamWriter.Close();
        notificationsStream.Close();
        Console.WriteLine("ErrorsLogs");
    }
    return;
}
Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken) //Ошибки
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    FileStream notificationsStream = new FileStream("D:\\Desktop\\krasota-Vanya\\ErrorLogs.TXT", FileMode.Append); // Логи

    string sMessage = ErrorMessage.ToString() + "\n   " + DateTime.Now;
    StreamWriter streamWriter = new StreamWriter(notificationsStream);

    streamWriter.WriteLine(sMessage);

    streamWriter.Close();
    notificationsStream.Close();
    Console.WriteLine("ErrorsLogs");
    return Task.CompletedTask;
}
async Task GetContactAsync(ITelegramBotClient botClient, MessageType messageType, Message message)//Телефон
{
    ReplyKeyboardRemove hide = new ReplyKeyboardRemove();
    //await botClient.SendTextMessageAsync(message.Chat.Id, $"You said:\n{message.Contact.PhoneNumber}");
    var username = message.Chat.Username;
    var name = message.Chat.FirstName + " " + message.Chat.LastName;
    var chatId = message.Chat.Id;
    var ss = message.Text;
    var telegramId = message.From.Id;
    var phoneNumber = (message.Contact.PhoneNumber).Replace("+", "").Remove(0, 1);
    try
    {
        await botClient.SendTextMessageAsync(chatId, $"Номер \\- _*{phoneNumber}*_", replyMarkup: hide, parseMode: ParseMode.MarkdownV2);
    }
    catch (Exception ex)
    {
        FileStream notificationsStream = new FileStream("C:\\Users\\Delfa\\Documents\\ErrorsLogs.txt", FileMode.Append); // Логи
        string sMessage = ex.ToString() + "\n\n" + DateTime.Now.ToString();
        StreamWriter streamWriter = new StreamWriter(notificationsStream);
        streamWriter.WriteLine(sMessage);
        streamWriter.Close();
        notificationsStream.Close();
        Console.WriteLine("ErrorsLogs");
    }
    return;
}
async Task GetLocationAsync(ITelegramBotClient botClient, MessageType messageType, Message message)//Местоположение
{
    ReplyKeyboardRemove hide = new ReplyKeyboardRemove();
    //await botClient.SendTextMessageAsync(message.Chat.Id, $"You said:\n{message.Contact.PhoneNumber}");
    var username = message.Chat.Username;
    var name = message.Chat.FirstName + " " + message.Chat.LastName;
    var chatId = message.Chat.Id;
    var ss = message.Text;
    var telegramId = message.From.Id;
    var latti = message.Location.Latitude;
    var longi = message.Location.Longitude;
    try
    {
        await botClient.SendTextMessageAsync(chatId, $"Местоположение \\- \nШирота: _{latti}_, Долгота: _{longi}_", replyMarkup: hide, parseMode: ParseMode.MarkdownV2);
    }
    catch (Exception ex)
    {
        FileStream notificationsStream = new FileStream("D:\\Desktop\\krasota-Vanya\\ErrorLogs.TXT", FileMode.Append); // Логи
        string sMessage = ex.ToString() + "\n\n" + DateTime.Now.ToString();
        StreamWriter streamWriter = new StreamWriter(notificationsStream);
        streamWriter.WriteLine(sMessage);
        streamWriter.Close();
        notificationsStream.Close();
        Console.WriteLine("ErrorsLogs");
    }
    return;
}
static async Task<DataTable> Select(string selectSQL)
{
    DataTable data = new DataTable("dataBase");

    //string path = "ConnectionString.txt";

    //string text = System.IO.File.ReadAllText(path);

    //string[] vs = text.Split('"');

    SqlConnection sqlConnection = new SqlConnection($"server=DESKTOP-80SJSE0;Trusted_connection=yes;DataBase=tester;User= ;PWD= ");
    sqlConnection.Open();

    SqlCommand sqlCommand = sqlConnection.CreateCommand();
    sqlCommand.CommandText = selectSQL;

    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
    sqlDataAdapter.Fill(data);

    sqlCommand.Dispose();
    sqlDataAdapter.Dispose();
    sqlConnection.Close();

    return data;
}//Запрос
