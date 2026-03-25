import { PUBLIC_API_BASE_URL } from '$env/static/public'

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

export async function safeFetch(url: string, init? : RequestInit) : Promise<Response>{
    let validValueRecieved = false;
    let response = new Response();
    while (!validValueRecieved){
        const token = sessionStorage.getItem('transfer') || 'default';

        const requestUrl = `${PUBLIC_API_BASE_URL}${url}`

        // force add the right headers for request
        if (init === null || init === undefined){
            init = {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json",
                    'Authorization': `Bearer ${token}`, 
                }
            }
        }
        else if (init.headers === null){
            init.headers = {
                "Content-Type": "application/json",
                "Accept": "application/json",
                'Authorization': `Bearer ${token}`, 
            }
        }
        else{
            init.headers = {
                ...init.headers,
                'Authorization': `Bearer ${token}`, 
            }
        }


        response = await fetch(requestUrl, init);

        if (response.status == 401){
            try{
                await renewToken();
            }
            catch{
                validValueRecieved = true;
            }
        }
        else{
            validValueRecieved = true;
        }
    }
    return response;
}

async function renewToken(){
    const url = `${PUBLIC_API_BASE_URL}/Auth/refresh`;


    const response = await fetch(url, {
        method : "POST",
        credentials : 'include',
        headers : {
            'Content-Type' : 'application/json'
        }
    });
    if (!response.ok){
        throw new Error("Failed to renew token")
    }
    const jwt : JWTResponse | null = await response.json();

    if (jwt === null){
        throw new Error("Token malformed")
    }

    sessionStorage.setItem("transfer", jwt.token)
    sessionStorage.setItem("uuid", jwt.uid)
    sessionStorage.setItem("isSuperuser", jwt.isSuper.toString())
}







