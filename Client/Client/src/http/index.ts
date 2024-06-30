import axios, { AxiosRequestConfig } from 'axios'
import { config } from 'process';
import {IAuthResponse} from "../models/AuthResponse";

export const API_URL = "http://localhost:5274/api";

const $api = axios.create({
    withCredentials: true,
    baseURL: API_URL
})


$api.interceptors.request.use((config)=>{
    config.headers.Authorization = `Bearer ${localStorage.getItem('token')}`;
    return config;
})

$api.interceptors.response.use((config)=>{
    return config;
},async (error)=>{
    const originalRequest = error.config;
    if(error.response.status === 401 && !error.config._isRetry){
        originalRequest._isRetry = true
        try{
            const response = await axios.post<IAuthResponse>(`${API_URL}/Participants/refresh-token`,{
                JwtToken:localStorage.getItem('token'),
                RefreshToken:""
            }, {withCredentials:true})
            localStorage.setItem('token',response.data.jwtToken);
            return $api.request(originalRequest);
        }catch(e:any){
            
            console.log('Пользователь не авторизован');
        
        }
    }
    throw error;
    
})

export default $api;