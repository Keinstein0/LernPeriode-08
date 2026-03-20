<script lang="ts">
  import { onMount } from 'svelte';
  import { safeFetch } from '../request';

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

  onMount(() => {
    uuid = sessionStorage.getItem('uuid') || 'default';
    token = sessionStorage.getItem('transfer') || 'default';
    isSuper = (sessionStorage.getItem('isSuperuser') || 'false') === 'true';
    
    userPromise = getUser(token);
    userListPromise = getUserList(token);
  });
</script>

{#await userPromise}
  <p>...loading user data</p>
{:then user}
    <div 
    class="user-image" 
    style="
        background-image: url('https://api.dicebear.com/9.x/thumbs/svg?seed={uuid}');
        background-color: {'white'};
    "
    ></div>
    <h1>Hello {user?.username}</h1>

    <div>
    <button>Update Account</button>
    <button>Delete Account</button>
    </div>

{:catch error}
  <p style="color: red;">{error.message}</p>
{/await}

{#await userListPromise}
    <p>Loading users</p>
{:then users} 
    <h1>Other Users</h1>

    {#each users || [] as user} 
        <div class="otheruser-div">
            <div class="otheruser-img"
                style="
                background-image: url('https://api.dicebear.com/9.x/thumbs/svg?seed={user.id}');
                background-color: {'white'};
                "
            ></div>
            <p>{user.username} {user.id == uuid ? '(you)' : ''}</p>
            {#if isSuper}
                <button>Delete</button>
                <button>Update</button>
            {/if}
        </div>
    {/each}
{:catch error}
    <p style="color: red;">{error.message}</p>
{/await}

{#if deleteConfirmationVisible}
    <div class="confirm-cancel">
        <h1>Are you sure?</h1>
        <p>This action cannot be undone!</p>
        <div>
            <button>Cancel</button>
            <button>Confirm</button>
        </div>
    </div>
{/if}

<style>
    p, h1{
        color: white;
    }

    .user-image {
        width: 240px; 
        height: 240px;
        display: inline-block;
        flex-shrink: 0;
        
        /* Make sure the image fits nicely */
        background-repeat: no-repeat;
        background-position: center;
        background-size: contain;
        
        /* Optional: if the PNG has transparency, 
        the background-color will show through the holes */
        border-radius: 50%; 
    }

    .otheruser-div{
        background-color: red;
        display: flex;
        flex-direction: row;
        align-content: center;
    }

    .otheruser-img{
        width: 32px;
        height: 32px;
        align-self: center;
        border-radius: 100px;
    }

    .confirm-cancel{
        position: absolute;
        padding: 20px;
        
        margin: auto;

        right: 0;
        left: 0;
        top: 0;
        bottom: 0;
        width: fit-content;
        height: fit-content;

        background-color: rgb(0, 4, 255);
    }





</style>