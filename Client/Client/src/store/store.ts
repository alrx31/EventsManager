import {IUser} from "../models/User";
import {makeAutoObservable} from "mobx";
import AuthService from "../services/AuthService";
import UserService from "../services/UserService";
import axios from "axios";
import {API_URL} from "../http";
import {IAuthResponse} from "../models/AuthResponse";
import EventsService from "../services/EventsService";
import {IEvent} from "../models/Event";

export default class Store {
    user = {} as IUser;
    isAuht = false;
    isLoading = false;


    constructor() {
        makeAutoObservable(this);
    }

    setAuth(bool: boolean) {
        this.isAuht = bool;
    }

    setUser(user: IUser) {
        this.user = user;
    }


    setLoading(bool: boolean) {
        this.isLoading = bool;
    }


    async login(email: string, password: string) {
        this.setLoading(true)
        try {
            const response = await AuthService.login(email, password);
            console.log(response)
            localStorage.setItem('token', response.data.jwtToken);
            this.setAuth(true);
            if (response.data.userId == 0) {
                throw 'Ошибка получения данных пользователя';
            }
            const res = await UserService.fetchUserById(response.data.userId);
            if (res.data) this.setUser(res.data);
            else console.log('Ошибка получения данных пользователя');

        } catch (e: any) {
            console.log(e.response?.data?.message);
        } finally {
            this.setLoading(false);
        }
    }

    async registration(
        email: string,
        password: string,
        firstName: string,
        lastName: string,
        BirthDate: Date
    ) {
        try {
            const response = await AuthService.register(
                email,
                password,
                firstName,
                lastName,
                BirthDate
            );
            if (response.status === 200) {
                alert('Успешная регистрация');
            }
        } catch (e: any) {
            console.log(e.response?.data?.message);
        }
    }

    async logout() {
        try {
            const response = await AuthService.logout();
            localStorage.removeItem('token');
            this.setAuth(false);
            this.setUser({} as IUser);
        } catch (e: any) {
            console.log(e.response?.data?.message);
        }
    }


    async checkAuth() {
        this.setLoading(true);
        try {
            const response = await axios.post<IAuthResponse>(`${API_URL}/Participants/refresh-token`, {
                JwtToken: localStorage.getItem('token'),
                RefreshToken: ""
            }, {withCredentials: true})
            console.log(response)

            localStorage.setItem('token', response.data.jwtToken);
            if (response.data.userId === 0) throw 'Ошибка получения данных пользователя';
            this.setAuth(true);
            
            const res = await UserService.fetchUserById(response.data.userId);
            console.log(res)
            if (res.data) this.setUser({
                Id: res.data.id,
                FirstName: res.data.firstName,
                LastName: res.data.lastName,
                Email: res.data.email,
                BirthDate: res.data.birthDate,
                RegisterationDate: res.data.registerationDate
            });
            else console.log('Ошибка получения данных пользователя');

        } catch (e: any) {
            console.log(e?.response?.data?.message);
        } finally {
            this.setLoading(false);
        }
    }
}