import $api from '../http'
import { AxiosResponse } from 'axios'
import {IAuthResponse, IPasswordResetResponse} from "../models/AuthResponse";

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
        LastName:string
    ):Promise<AxiosResponse<IAuthResponse>>{
        // BirthDate требуется сервером, отправляем дефолтное значение
        const defaultBirthDate = new Date();
        defaultBirthDate.setFullYear(defaultBirthDate.getFullYear() - 18);
        
        return $api.post<IAuthResponse>('/Participants/register', {
            FirstName,
            LastName,
            BirthDate: defaultBirthDate,
            RegistrationDate: new Date(),
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
    
    static async resetPassword(email: string): Promise<AxiosResponse<IPasswordResetResponse>> {
        return $api.post<IPasswordResetResponse>('/Participants/reset-password', {
            Email: email
        })
    }
}