import {IUser} from "./User";

export interface IAuthResponse {
    isLoggedIn:boolean;
    userId:number;
    jwtToken:string;
    refreshToken:string;
}

export interface IPasswordResetResponse {
    success: boolean;
    message: string;
}