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

#### PUT /auth/{uid}
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

#### GET /playlist
*gets all of your playlists*

---------------

#### GET /playlist/{id}
*gets single playlist with all the songs*


## Songs
#### GET /music?page=0&length=10
*gets all music in pages*

----------

#### GET /music/{id}
*gets single song fully*

-----------

## Songs upload
#### POST /music
```
{
    srcUrl : https://youtu.be/23lo23gc //where its fetched from
}
```
*fetches the music and returns the metadata including the upload id*

--------

#### POST /music/{id}
```
{
    title : myChangedTitle,
    composer : myChangedComposer,
    thumbnail : myUpdatedThumbnail
}
```
*second step to uploading, uploads the fetched file to S23 bucket with corrected metadata*

--------

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