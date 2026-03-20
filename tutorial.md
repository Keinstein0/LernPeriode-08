# Tutorial for Svelte

## 1. What is svelte?
Svelte is a modern alternative to plain HTML JS. In contrast to many other js frameworks it writes in it's own files which then get compiled to high performing vanillia HTML CSS JS code. This makes it really optimized. Many of it's development features allow developers to develop their apps quickly and efficiently. 

## 2. How does it work?
### Installing
For our purposes we'll make a Website. In order to start off we'll first have to install svelte in the first place. You can do that via npm. (Official installation guide: https://svelte.dev/docs/svelte/getting-started)

### Getting Started
In the routes directory you should now see two files: +layout.svelte and +page.svelte. By opening +page.svelte you can start to write what you want the user to see when he/she enters your site. Let's write a simple Hello-World to demonstrate that it's working. Paste the following code:
```html
<h1>Hello World from Svelte!</h1>
```
As you can see it's just like plain HTML but with many more features you'll see later on. But lets take a look at the website in the first place. For this you simply need to run ```npm run dev``` in your terminal and it'll start your website. It should give you a url to go to once it's started and there you should see your Hello-World message from before. 

### Runes 101

Lets say we wanted to write a script that writes out whatever you put into a variable you need to use runes. They tell svelte "hey that's like this upon initialisation".
```html
<script lang=js>
    const name = $state("Me")

</script>

<h1>Hello, my name is {name}</h1>
```
And simply by setting curly brackets you can replace the content of the element. No need for getting the element by id or changing the inner HTML of an element. 

### {#if} {#each} {#await}
Svelte actually introduces logic components to your else static html. They can be seen by the curly brackets in contrast to the <> from regular HTML tags. There's multiple types that allow you to do different things

#### {#if}
Using this block we can show/hide certain blocks under a condition. Lets say we wanted to hide a block that tells the user his name until a number is greater then 10
```html
<script lang=js>
    const showUserName = $state(false)
</script>

{#if showUserName}
    <h1>Hello User</h1>
{/if}
```

#### {#for}
For is used for iterating through entries in an array. For example lets say you wanted to display every item in a shopping cart:
```html
<script lang=js>
    const items = $state([apple, banana])
</script>

{#each items as item}
    <h1>Item: {item}</h1>
{/each}
```

#### {#await}
You can also await tasks in the code directly. Lets say you had to wait for your data to be loaded first before showing it:
```html
<script lang=js>
    import getUserdata from dataPickuper;

    let userData = getUserData()
</script>

{#await userData}
    <h1>Loading</h1>
{:then}
    <h1>userData<h1>
{:catch error}
    <p style="color: red;">{error.message}</p>
{/await}
```

There's plenty of more features that come in useful but i don't have the time to explain every single one of them. If you wis hfor a full tutorial then i'd recomment the official one: https://svelte.dev/tutorial/svelte/welcome-to-svelte
