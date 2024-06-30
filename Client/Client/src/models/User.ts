export interface IUser{
    id:number;
    firstName:string;
    lastName:string;
    email:string;
    birthDate:Date;
    registerationDate:Date;
}
export interface IUserLogin extends IUser{
    Password:string;
}