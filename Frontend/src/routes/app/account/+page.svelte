<script lang="ts">
  import { onMount } from 'svelte';

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
  
  let userPromise = $state<Promise<User | null>>(new Promise(() => {}));

  async function getUser(currentToken: string | null): Promise<User | null> {
    if (!currentToken || currentToken === 'default') {
       return null; 
    }

    const response = await fetch("http://localhost:5016/api/Auth", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        "Accept": "application/json",
        'Authorization': `Bearer ${token}`, 
      }
    });

    if (!response.ok) throw new Error('Failed to fetch user');

    const data = await response.json();
    return new User(data.id, data.username, data.isSuper);
  }

  onMount(() => {
    uuid = sessionStorage.getItem('uuid') || 'default';
    token = sessionStorage.getItem('transfer') || 'default';
    isSuper = (sessionStorage.getItem('isSuperuser') || 'false') === 'true';
    
    userPromise = getUser(token);
  });
</script>

{#await userPromise}
  <p>...loading user data</p>
{:then user}
  <h1 style="color: white">Hello {user?.username}</h1>
{:catch error}
  <p style="color: red;">{error.message}</p>
{/await}

<div>Image</div>

<div>
    <button>Update Account</button>
    <button>Delete Account</button>
</div>

<h2>Other Users</h2>
<div>

</div>

<style>
    h1{
        color: white;
    }

</style>