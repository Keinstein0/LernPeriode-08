# Endpoints:
## User/Auth
#### POST /auth *#IsSuper*
```
{
    username : username,
    password : password
}
```
*returns uid*

---------------

#### PUT /auth/{uid} *(#IsSuper)*
```
{
    username : username,
    password : password
}
```
*updates the user with uid, if super can update any, else only yours*

---------------

#### DELETE /auth/{uid}
*deletes user with uid*

---------------

#### POST /auth/{uid}/login
```
{
    username : username,
    password : password
}
```
*returns a jwt and if the user is super and uid*

---------------

#### POST /auth/{uid}/refresh
*returns a newly valid jwt*

---------------

#### GET /auth 
*returns all users and their uid*

---------------

#### GET /auth/{uid}
*returns single user with id*

---------------

## Playlists
### Playlist
#### POST /playlist
```
{
    name : name,
}
```
*creates a new playlist and returns id*

---------------

#### PUT /playlist/{id}
```
{
    name : name
}
```
*updates playlist name*

---------------

#### DELETE /playlist/{id}
*deletes playlist*

---------------

#### GET /playlist *?filter=abc*
*gets all of your playlists (optionally filtered by name)*

---------------

#### GET /playlist/{list_id}
*gets single playlist with all the songs and their indices*

---------------

### Playlist:Songs

#### PUT /playlist/{playlistId}/{musicId}
*updates the index of the song, cascades all other songs after to adapt their indices*

#### DELETE /playlist/{playlistId}/{musicId}
*deletes the song from the playlist, cascades all other index songs*

#### POST /playlist/{playlistId}/{musicId}
*appends a song to the end of the playlist*



## Songs
### Songs
#### GET /music?*page=0&length=10&filter=abc*
*gets all music in pages*

----------

#### GET /music/{id}
*gets single song fully*

-----------


#### PUT /music/{id}
```
{
    title : myChangedTitle,
    composer : myChangedComposer,
    thumbnail : myUpdatedThumbnail
}
```
*updated metadata on a piece of music*

--------

#### DELETE /music/{id} *#IsSuper*
*deletes a piece of music*

--------

### Songs:Upload
#### POST /music
```
{
    srcUrl : https://youtu.be/23lo23gc //where its fetched from
    title : myChangedTitle, (opt)
    composer : myChangedComposer, (opt)
    thumbnail : myUpdatedThumbnailFilestream (opt)
}
```
*fetches the music, updates metadata on demand and returns the the upload id*

--------