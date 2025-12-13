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
    pageSize = 5;

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


    async login(email: string, password: string): Promise<{success: boolean; errorType?: string}> {
        this.setLoading(true)
        try {
            const response = await AuthService.login(email, password);
            
            // Проверяем успешность логина - если нет токена или isLoggedIn = false
            if (!response.data || !response.data.jwtToken || !response.data.isLoggedIn) {
                return { success: false, errorType: 'invalid_credentials' };
            }
            
            localStorage.setItem('token', response.data.jwtToken);
            this.setAuth(true);
            
            if (!response.data.userId || response.data.userId === 0) {
                return { success: false, errorType: 'user_not_found' };
            }
            
            const res = await UserService.fetchUserById(response.data.userId);
            if (res.data) {
                this.setUser(res.data);
                return { success: true };
            } else {
                console.log('Ошибка получения данных пользователя');
                return { success: false, errorType: 'user_not_found' };
            }

        } catch (e: any) {
            console.log('Login error:', e.response?.status, e.response?.data);
            
            // Определяем тип ошибки по статус-коду
            const status = e.response?.status;
            if (status === 401 || status === 400) {
                // 401 Unauthorized или 400 Bad Request - неверные учетные данные
                return { success: false, errorType: 'invalid_credentials' };
            } else if (status === 404) {
                return { success: false, errorType: 'user_not_found' };
            }
            
            return { success: false, errorType: 'server_error' };
        } finally {
            this.setLoading(false);
        }
    }

    async registration(
        email: string,
        password: string,
        firstName: string,
        lastName: string
    ): Promise<{success: boolean; errorType?: string}> {
        try {
            const response = await AuthService.register(
                email,
                password,
                firstName,
                lastName
            );
            if (response.status === 200 || response.status === 201) {
                return { success: true };
            }
            return { success: false, errorType: 'server_error' };
        } catch (e: any) {
            console.log('Registration error:', e.response?.status, e.response?.data);

            const status = e.response?.status;
            const rawMessage = e.response?.data?.message || e.response?.data?.Message || '';
            const message = rawMessage.toLowerCase();
            const isEmailExistsMessage = message.includes('email') ||
                message.includes('exist') ||
                message.includes('already') ||
                message.includes('существует');

            if (status === 409 || status === 400 || isEmailExistsMessage) {
                if (isEmailExistsMessage) {
                    return { success: false, errorType: 'email_exists' };
                }
                return { success: false, errorType: 'invalid_data' };
            }

            // even if 500, try to surface specific message about email
            if (status === 500 && isEmailExistsMessage) {
                return { success: false, errorType: 'email_exists' };
            }

            return { success: false, errorType: 'server_error' };
        }
    }

    async logout() {
        try {
            const response = await AuthService.logout(this.user.id);
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

            localStorage.setItem('token', response.data.jwtToken);
            if (response.data.userId === 0) throw 'Ошибка получения данных пользователя';
            this.setAuth(true);
            
            const res = await UserService.fetchUserById(response.data.userId);
            if (res.data) this.setUser({
                id: res.data.id,
                firstName: res.data.firstName,
                lastName: res.data.lastName,
                email: res.data.email,
                birthDate: res.data.birthDate,
                registrationDate: res.data.registrationDate,
                isAdmin: res.data.isAdmin
            });
            else console.log('Ошибка получения данных пользователя');

        } catch (e: any) {
            console.log(e?.response?.data?.message);
        } finally {
            this.setLoading(false);
        }
    }
}