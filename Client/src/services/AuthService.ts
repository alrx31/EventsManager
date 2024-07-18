import $api from '../http'
import { AxiosResponse } from 'axios'
import {IAuthResponse} from "../models/AuthResponse";
import Store from "../store/store";

export default class AuthService{
    static async login(
        email:string,
        password:string
    ):Promise<AxiosResponse<IAuthResponse>>{
        return $api.post<IAuthResponse>('/Participants/login', {
            Email:email,
            Password:password
        })
    }
    static async register(
        email:string,
        password:string,
        FirstName:string,
        LastName:string,
        BirthDate:Date
    ):Promise<AxiosResponse<IAuthResponse>>{
        return $api.post<IAuthResponse>('/Participants/register', {
            FirstName,
            LastName,
            BirthDate,
            RegistrationDate:new Date(),
            email,
            password
        })
    }

    static async logout(userId:number):Promise<AxiosResponse<IAuthResponse>>{
            let ask = {
                "Token":localStorage.getItem('token') ? localStorage.getItem('token') : "",
                "UserId":userId ? userId : 0
            }
        return $api.post<IAuthResponse>('/Participants/logout', ask)
    }
}