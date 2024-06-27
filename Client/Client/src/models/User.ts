export interface IUser{
    Id:number;
    FirstName:string;
    LastName:string;
    Email:string;
    BirthDate:Date;
    RegisterationDate:Date;
}
export interface IUserLogin extends IUser{
    Password:string;
}