
# Token Information

This document will give you information on what a token is and why you need to put your token in for it to work.


## What is a token

A token is Discord's way of authenticating who is performing an action. Whenever you make an API request, your token is sent to make sure it's valid and it knows who is performing the action.
## Why does the app need your token

The app needs your token to be able to send requests as you as a user, without your token, we'd have no way to make sure the request has authentication and changes your status.
## Is there a security issue?

Well you can view the source code, but there's mostly no security issue. The only way you can get your token grabbed from this program is if you have a virus downloaded, since it saves your token in a file for easy access.
## How to get your token?

Glad you asked! It's extremely simple. Firstly, you need to go to [discord](https://discord.com/app). Once there, open Dev Tools -> Console. Inside the console just paste the following code snippet:
```js
webpackChunkdiscord_app.push([[Math.random()],{},(r)=>{console.log(Object.values(r.c).find(m=>m.exports&&m.exports.default&&m.exports.default.getToken!==void 0).exports.default.getToken())}]);
```
This will print out your token, just copy this and paste it in the token slot, once you do your first start, the program should save your token and allow easy usage for any following attempts.