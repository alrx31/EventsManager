export interface IUser{
    id:number;
    firstName:string;
    lastName:string;
    email:string;
    birthDate:Date;
    registrationDate:Date;
    isAdmin:boolean;
}
export interface IUserLogin extends IUser{
    Password:string;
}