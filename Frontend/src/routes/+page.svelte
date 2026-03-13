<script lang="ts">
    import { goto } from '$app/navigation';

    const BASE_URL = "http://localhost:5016/api";
    const LOGIN_ROUTE = "/Auth/login";

    class JWTResponse{
        token: string;
        uid: string;
        isSuper: boolean;

        constructor(token: string, uid: string, isSuper: boolean){
            this.token = token;
            this.uid = uid;
            this.isSuper = isSuper;
        }
    }


    let username : string = $state("");
    let password : string = $state("");
    let jwt : JWTResponse | null = $state(null);

    let showPassword : boolean = $state(false);
    let isUnauthorized : boolean = $state(false);

    async function logIn(){
        const url = BASE_URL + LOGIN_ROUTE;
        const body = {
            username : username,
            password : password
        }
        try{
            const response = await fetch(url, {
                method : "POST",
                body: JSON.stringify(body),
                headers: {
                "Content-Type": "application/json", // This is the missing piece!
                "Accept": "application/json"}
            })
            if (response.status === 400){
                isUnauthorized = true;
                return;
            }
            else if (!response.ok){
                throw new Error('Network response was not ok');
            }
            jwt = await response.json();
            if (!jwt){
                isUnauthorized = true;
                return;
            }

            sessionStorage.setItem("transfer", jwt.token)
            sessionStorage.setItem("uuid", jwt.uid)
            sessionStorage.setItem("isSuperuser", jwt.isSuper.toString())

            goto("/app/search");
        }
        catch (err){
            console.error(err)
        }
    }
    function togglePasswordVisibility(){
        showPassword = !showPassword;
    }
</script>


<div class=login-page>
    <div class="container">
        <h1>Log in</h1>

        {#if isUnauthorized}
            <p class="error">Invalid Credentials</p>
        {/if}

        <input type="text" bind:value={username} class="no-border" placeholder="username">
        <div class="input-group">
            {#if showPassword}
            <input type="text" bind:value={password} class="no-border" placeholder="password">
        {:else}
            <input type="password" bind:value={password} placeholder="password">
        {/if}
            <button type="button" class="pass-reveal" onclick={togglePasswordVisibility}>
                {#if showPassword}
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-eye" viewBox="0 0 16 16">
                    <path d="M16 8s-3-5.5-8-5.5S0 8 0 8s3 5.5 8 5.5S16 8 16 8zM1.173 8a13.133 13.133 0 0 1 1.66-2.043C4.12 4.668 5.88 3.5 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.133 13.133 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755C11.879 11.332 10.119 12.5 8 12.5c-2.12 0-3.879-1.168-5.168-2.457A13.134 13.134 0 0 1 1.172 8z"/>
                    <path d="M8 5.5a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5zM4.5 8a3.5 3.5 0 1 1 7 0 3.5 3.5 0 0 1-7 0z"/>
                </svg>
                {:else}
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-eye-slash" viewBox="0 0 16 16">
                    <path d="M13.359 11.238C15.06 9.72 16 8 16 8s-3-5.5-8-5.5a7.028 7.028 0 0 0-2.79.588l.77.771A5.944 5.944 0 0 1 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.134 13.134 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755-.165.165-.337.328-.517.486l.708.709z"/>
                    <path d="M11.297 9.176a3.5 3.5 0 0 0-4.474-4.474l.823.823a2.5 2.5 0 0 1 2.829 2.829l.822.822zm-2.943 1.299.822.822a3.5 3.5 0 0 1-4.474-4.474l.823.823a2.5 2.5 0 0 0 2.829 2.829z"/>
                    <path d="M3.35 5.47c-.18.16-.353.322-.518.487A13.134 13.134 0 0 0 1.172 8l.195.288c.335.48.83 1.12 1.465 1.755C4.121 11.332 5.881 12.5 8 12.5c.716 0 1.39-.133 2.02-.36l.77.772A7.029 7.029 0 0 1 8 13.5C3 13.5 0 8 0 8s.939-1.721 2.641-3.238l.708.709zm10.296 8.884-12-12 .708-.708 12 12-.708.708z"/>
                </svg>
                {/if}
            </button>
        </div>

        <button class="submit-button" onclick={logIn}>Login</button>
    </div>
</div>

<style>

    :global(body) {
        margin: 0;
    }

    .login-page {
        min-height: 100vh;
        display: flex;
        justify-content: center;
        align-items: center;
        width: 100%;
        font-family: Arial, Helvetica, sans-serif;
        background-image: url('/Images/layered-peaks-haikei.svg');
        background-size: auto, 100%;
    }

    .input-group {
        display: flex;
        border: 2px solid black;
        border-radius: 10px;
        background-color: white; 
        overflow: hidden; 
    }

    .input-group input {
        border: none;
        flex-grow: 1; 
        outline: none; 
    }

    .pass-reveal {
        border: none;
        border-left: 2px solid black; 
        border-radius: 0;
        background-color: #eee;
        cursor: pointer;
        display: flex;
        align-items: center;
        padding: 0 10px;
    }

    input {
        min-height: 30px;
        border: 2px solid black;
        border-radius: 10px;
        padding: 5px; 
        min-width: 35vw;
    }

    .container{
        background-color: rgba(43, 43, 43, 0.845);
        border: 5px solid rgb(0, 0, 0);
        border-radius: 10px;
        padding: 5vw;
        margin-left: 2%;
        margin-right: 2%;

        display: flex;
        flex-direction: column;
        gap: 10px;

    }

    .submit-button{
        align-self:center;
        width: 15vw;
        height: 30px;
        transition: 200ms;
        border-radius: 10px;
        font-size: larger;
    }
    .submit-button:hover{
        background-color: rgb(158, 158, 255);
    }

    h1{
        color:rgb(200, 200, 200);
    }

    .error{
        color: red;
        font-size: large;
        margin: 0;
    }

</style>