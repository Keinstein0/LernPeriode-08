<script lang="ts">
  import { onMount } from 'svelte';
  import { safeFetch } from '../request';
  import Passwordfield from './passwordfield.svelte';

  class User {
    id: string;
    username: string;
    isSuper: boolean;

    constructor(id: string, username: string, isSuper: boolean) {
      this.id = id;
      this.username = username;
      this.isSuper = isSuper;
    }
  }

  let uuid = $state<string | null>(null);
  let token = $state<string | null>(null);
  let isSuper = $state<boolean | null>(null);

  let deleteConfirmationVisible = $state<boolean>(false);
  let updateInputVisible = $state<boolean>(false);

  let username = $state<string>();
  let password = $state<string>();
  let confirmPwd = $state<string>();
  let error = $state<string>("");

  let createUserVisible = $state<boolean>(false);


  let editId = $state<string | null>(null);
  
  let userPromise = $state<Promise<User | null>>(new Promise(() => {}));
  let userListPromise = $state<Promise<Array<User> | null> | null | undefined>();


  async function getUser(currentToken: string | null): Promise<User | null> {
    if (!currentToken || currentToken === 'default') {
       return null; 
    }

    const response = await safeFetch(`/Auth/${uuid}`);

    if (!response.ok) throw new Error('Failed to fetch user');

    const data = await response.json();
    return new User(data.id, data.username, data.isSuper);
  }

  async function getUserList(currentToken: string | null): Promise<Array<User> | null>{
    if (!currentToken || currentToken === 'default') {
       return null; 
    }
    const response = await safeFetch("/Auth");
    if (!response.ok) throw new Error('failed to fetch users')


    const data = await response.json();

    const list = data.map((u: any) => new User(u.id, u.username, u.isSuper));
    console.log(list);
    return list
  }


  async function showDeleteMenu(menuId : string | null){
    if (menuId === null){
        return;
    }

    editId = menuId;
    deleteConfirmationVisible = true;
  }

  async function showUpdateMenu(menuId : string | null){
    if (menuId === null){
        return;
    }

    editId = menuId;
    error = "";
    updateInputVisible = true;
  }

  async function deleteUser(){
    if (editId === null){
        error = "Passwords not matching";
        return;
    }

    const response = await safeFetch(`/Auth/${editId}`,{
        "method" : "DELETE"
    })
    if (!response.ok){
        error = await response.text();
        throw new Error('failed to delete user')
    }

    userPromise = getUser(token);
    userListPromise = getUserList(token);

    deleteConfirmationVisible = false;
    // delete user via DELETE /Auth/{id}
  }
  async function updateUser(){
    error = "";
    if (editId === null){
        updateInputVisible = false;
        return;
    }

    if (!(password === confirmPwd)){
        error = "passwords not matching"
        return;
    }

    const body = {
        username : username,
        password : password
    }

    const response = await safeFetch(`/Auth/${editId}`, {
        method : "PUT",
        body : JSON.stringify(body),
        headers : {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    })

    if (!response.ok){
        error = await response.text();
        throw new Error('failed to update user')
    }

    userPromise = getUser(token);
    userListPromise = getUserList(token);

    updateInputVisible = false;
  }

  async function createUser(){
    error = "";

    if (!(password === confirmPwd)){
        error = "passwords not matching"
        return;
    }

    const body = {
        username : username,
        password : password
    }

    const response = await safeFetch(`/Auth`, {
        method : "POST",
        body : JSON.stringify(body),
        headers : {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    })

    if (!response.ok){
        error = await response.text();
        throw new Error('failed to update user')
    }

    userPromise = getUser(token);
    userListPromise = getUserList(token);

    createUserVisible = false;
  }

  async function onKeyDown(key : KeyboardEvent){
    if (key.key == "Escape"){
        createUserVisible = false;
        updateInputVisible = false;
        deleteConfirmationVisible = false;
    }
  }

  onMount(() => {
    uuid = sessionStorage.getItem('uuid') || 'default';
    token = sessionStorage.getItem('transfer') || 'default';
    isSuper = (sessionStorage.getItem('isSuperuser') || 'false') === 'true';
    
    userPromise = getUser(token);
    userListPromise = getUserList(token);
  });
</script>

<div class="main-container">
    {#await userPromise}
    <p>...loading user data</p>
    {:then user}
        <div class="user-image-wrapper">
            <div 
            class="user-image" 
            style="
                background-image: url('https://api.dicebear.com/9.x/thumbs/svg?seed={uuid}');
                background-color: {'white'};
            "
            ></div>
        </div>
        <div class="welcome-wrapper">
        <h1 class="welcome">Hello {user?.username} and welcome to Spotifake</h1>
        </div>

        <div class="youraccount-wrapper">
            <button onclick={() => showUpdateMenu(uuid)} class="youraccount-edit youraccount-update">Update Account</button>
            <button onclick={() => showDeleteMenu(uuid)} class="youraccount-edit youraccount-delete">Delete Account</button>
        </div>

    {:catch error}
    <p style="color: red;">{error.message}</p>
    {/await}

    {#await userListPromise}
        <p>Loading users</p>
    {:then users} 
        <h1>Other Users</h1>
        <div class="otherusers">
            {#each users || [] as user} 
                <div class="otheruser-div">
                    <div class="otheruser-img"
                        style="
                        background-image: url('https://api.dicebear.com/9.x/thumbs/svg?seed={user.id}');
                        background-color: {'white'};
                        "
                    ></div>
                    <p class="username-cell">{user.username} {user.id == uuid ? '(you)' : ''}</p>
                    <div class="action-cell">
                        {#if isSuper}
                            <button onclick={() => showDeleteMenu(user.id)}>Delete</button>
                        {/if}
                    </div>
                    <div class="action-cell">
                        {#if isSuper}
                            <button onclick={() => showUpdateMenu(user.id)}>Update</button>
                        {/if}
                    </div>
                </div>
            {/each}
        </div>
        {#if isSuper}
            <button onclick={() => createUserVisible = true} class="create-button">New User</button>
        {/if}
    {:catch error}
        <p style="color: red;">{error.message}</p>
    {/await}

    {#if deleteConfirmationVisible}
        <div class="confirm-cancel-wrapper">
            <div class="confirm-cancel">
                <h1 class="popup-header">Are you sure?</h1>
                <p>This action cannot be undone!</p>
                <p class="uuid-text">(for uuid {editId})</p>
                <div class="button-div">
                    <button class="overlay-button" onclick={() => deleteConfirmationVisible = false}>Cancel</button>
                    <button class="overlay-button" onclick={deleteUser}>Confirm</button>
                </div>
            </div>
        </div>
    {/if}

    {#if updateInputVisible}
        <div class=confirm-cancel-wrapper>
            <div class="confirm-cancel">
                <h1 class="popup-header">Change Username/Password</h1>
                <p class="uuid-text">(for uuid {editId})</p>
                <input type="text" placeholder="new username" bind:value={username}> <!--TODO: actual component for pwds-->
                <Passwordfield placeholder="new password" bind:value={password}></Passwordfield>
                <Passwordfield placeholder="confirm password" bind:value={confirmPwd}></Passwordfield>
                {#if error !== ""}
                    <p>{error}</p>
                {/if}
                <div class="button-div">
                    <button class="overlay-button" onclick={() => updateInputVisible = false}>Cancel</button>
                    <button class="overlay-button" onclick={updateUser}>Update</button>
                </div>
            </div>
        </div>
    {/if}

    {#if createUserVisible}
        <div class="confirm-cancel-wrapper">
            <div class="confirm-cancel">
                <h1 class="popup-header">Create new User</h1>
                <input type="text" placeholder="username" bind:value={username}>
                <Passwordfield placeholder="password" bind:value={password}></Passwordfield>
                <Passwordfield placeholder="confirm password" bind:value={confirmPwd}></Passwordfield>
                <div class="button-div">
                    <button class="overlay-button" onclick={() => createUserVisible=false}>Cancel</button>
                    <button class="overlay-button" onclick={createUser}>Create</button>
                </div>
            </div>
        </div>
    {/if}
</div>

<svelte:window on:keydown={onKeyDown} />

<style>
    p, h1{
        color: white;
    }
    @font-face{
        font-family: TangoSans;
        src: url('/Fonts/TangoSans.ttf');
    }

    .main-container{
        overscroll-behavior-y: none;
        height: 100%;
        font-family: TangoSans;
        overflow: hidden;
    }

    .user-image {
        width: 240px; 
        height: 240px;

        max-height: 20vh;
        max-width: 20vh;

        
        display: inline-block;
        flex-shrink: 0;
        
        background-repeat: no-repeat;
        background-position: center;
        background-size: contain;
        
        border-radius: 50%;
        outline: 5px solid #1ed760;
        outline-offset: 4px;

        transition: transform 600ms;
    }

    .user-image:hover{
        transform: rotate(360deg);
    }

    .user-image-wrapper{
        display: flex;
        justify-content: center;
        margin-top: 3vh;
        margin-bottom: 3vh;
    }

    .welcome-wrapper{
        display: flex;
        justify-content: center;
    }

    .welcome{
        text-align: center;
        transition: 500ms;
    }
    .welcome:hover{
        color: #65de90;
    }

    .youraccount-wrapper{
        display: flex;
        justify-content: center;
        gap: 7vw;
    }

    .youraccount-edit{
        border-radius: 1000px;
        padding-left: 5vw;
        padding-right: 5vw;
        padding-top: 1vh;
        padding-bottom: 1vh;

        background-color: #1ed760;
        color: white;

        outline: 2px solid rgb(255, 255, 255);
        outline-offset: 2px;
        border: none;

        font-family: TangoSans;

        transition: 50ms;
        cursor: pointer;
    }

    .youraccount-update{
        padding-left: 7.5vw;
        padding-right: 7.5vw;
        padding-top: 1vh;
        padding-bottom: 1vh;
    }

    .youraccount-delete{
        padding-left: 2.5vw;
        padding-right: 2.5vw;
        padding-top: 1vh;
        padding-bottom: 1vh;

        background-color: rgb(156, 0, 0);
    }

    .youraccount-edit:hover{
        outline: 3px solid rgb(155, 155, 155);
    }


    .otherusers{
        max-height: 25vh;
        overflow-y: auto;
        margin: 10px;

        scrollbar-width: thin;
        scrollbar-color: rgba(255, 255, 255, 0.243) #000000;

        display: grid;
        grid-template-columns: auto 1fr auto auto; 
        gap: 1rem;
        align-items: center;
        padding: 1rem;
        background-color: #121212;
        border-radius: 8px;
    }


    .otheruser-div{
        display: contents;
    }

    .otheruser-img{
        width: 32px;
        height: 32px;
        border-radius: 100px;
        background-size: cover;

        transition: transform 600ms;
    }

    .otheruser-img:hover{
        transform: rotate(360deg);
    }

    .username-cell{
        white-space: normal; 
        overflow-wrap: break-word;
        word-break: break-word;
        
        margin: 0;
        padding: 0 10px;
        align-self: center;
    }

    .action-cell {
        /* Ensures the cell exists even if the button is hidden */
        display: flex;
        justify-content: center;
        min-width: 80px; 
    }

    /* Optional: styling the buttons to look like Spotify's UI */
    .action-cell button {
        background: transparent;
        color: #b3b3b3;
        border: 1px solid #b3b3b3;
        border-radius: 4px;
        padding: 6px 10px;
        cursor: pointer;
        font-size: 0.8rem;
    }

    .action-cell button:hover {
        color: white;
        border-color: white;
    }


    .confirm-cancel-wrapper{
        background-color: rgba(35, 35, 35, 0.25);

        backdrop-filter: blur(6px);
        -webkit-backdrop-filter: blur(6px); 

        z-index: 99;
        position: absolute;
        top: 0;
        left: 0;

        width: 100vw;
        height: 100vh;

        display: flex;
        align-content: center;
        justify-content: center;
    }

    .confirm-cancel{
        padding: 20px;
        margin: auto;

        bottom: 0;
        width: fit-content;
        height: fit-content;
        max-width: 80vw;

        background-color: rgba(26, 26, 26, 0.8); 

        backdrop-filter: blur(12px);
        -webkit-backdrop-filter: blur(12px); 
        border-radius: 20px;
        border: 2px solid #1ed760;

        box-shadow: 0 8px 32px rgba(0, 0, 0, 0.5);
        z-index: 999;
    }

    .uuid-text{
        font-size: 8px;
        margin-bottom: 10px;
    }

    .popup-header{
        margin-bottom: 2px
    }

    .button-div{
        display: flex;
        flex-direction: row;
        justify-content: space-between;
    }

    .overlay-button{
        margin: 6px;
        margin-left: 3vw;
        margin-right: 3vw;

        border-radius: 4px;
        background: transparent;
        color: #b3b3b3;
        border: 1px solid #b3b3b3;
        border-radius: 4px;
        padding: 6px 14px;
        cursor: pointer;
        font-size: 0.8rem;
    }

    .overlay-button:hover{
        color: white;
        border-color: white;
    }




    .create-button{
        background: transparent;
        color: #b3b3b3;
        border: 1px solid #b3b3b3;
        border-radius: 4px;
        padding: 6px 5vw;
        cursor: pointer;
        font-size: 0.8rem;

        margin-top: 2vh
    }
    .create-button:hover {
        color: white;
        border-color: white;
    }




</style>