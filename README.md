# online-avalon
An online version of the board game Avalon

## Dependencies
- .NET Core 3.1
- Vue CLI 3.1

## Design Philosophies
- The client is never sent information that the current player isn't allowed to know
   - This may mean that the client might have to infer information based on what is known to make a more functional UI (*ex: if Percival is only sent one username, the UI can infer that the one username is Merlin*)
- The server is the one source of truth.
  - Since clients aren't sent more info than the user is allowed to know, the server is responsible for running the game and keeping track of all information.
