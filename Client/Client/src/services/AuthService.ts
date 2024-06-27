import $api from '../http'
import { AxiosResponse } from 'axios'
import {IAuthResponse} from "../models/AuthResponse";

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

    static async logout():Promise<AxiosResponse<IAuthResponse>>{
        return $api.post<IAuthResponse>('/logout')
    }
}