## Introduction

Here is a repository for PoC of chat using ASP.NET MVC and SignlaR library on both sides: backend and front-end.
On the backend side I created a class inherited from `Microsoft.AspNet.SignalR.Hub` library base class to handle all real-time requests from front-end.
In current implementation there is `SiemplifyChatHub.cs` class which handles sending new messages and connection of new clients to hub. In Connect() method it adds contacts for currently connected client and
loads history of messages.

## Testing
- Run several instances of application from Visual Studio and then enter name of user which will connect to chat
- After connection to chat all online contacts will be loaded to contacts list
- There is also 'All' link which provides ability to send messages to all users at once
- If user already registered (after first login with a name) then she will get messages after connection, even
if it was not connected. 
- All data stores in App_Data\chat-data.xml file for simplicity as it is PoC. There are all registered users and
history of messages.

## How it works? 
- All public methods in `SiemplifyChatHub` class are avaialble in front-end by using jquery.signalrR library.
- On front-end side you could register js-callbacks for backend using such code as an example:

>     var chat = $.connection.siemplifyChatHub;
>     // ...
>     chat.client.addNewMessageToPage = function (name, to, message, time) {
>     		var contactName = '@Model.Contact.Name';
>      	if (to == "All" && contactName != 'All') {
>                         return;
>               // Add the message to the page.
>               $('#discussion').append('<li>' + time + ': <b>' + htmlEncode(name)
>                         + ' to ' + htmlEncode(to) + '</b>: ' + htmlEncode(message) + '</li>');
>                 };

Take a look at Views\Chat.cshtml for more examples. Then this callback will be available in backend  through Hub-derived class.

- To call backend method from front-end use such code as an example:

>   var chat = $$.connection.siemplifyChatHub;
>  chat.server.connect($('#displayname').val(), $('#activecontact').val());

## Possible improvements

- Using Angular 7 for UI. For now, there is Razor for PoC 
- Improve UI to be more closer for mockup
- Replace XML with SQL Server database

#### Related links
- [# Tutorial: Real-time chat with SignalR 2 and MVC 5](https://docs.microsoft.com/en-us/aspnet/signalr/overview/getting-started/tutorial-getting-started-with-signalr-and-mvc#get-the-code)
- https://codingblast.com/asp-net-core-signalr-simple-chat/
- https://www.codementor.io/ibrahimsuta/how-to-build-a-simple-chat-using-asp-net-core-signalr-and-angular-5-dvonj1tu8
