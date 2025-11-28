import React, {useContext, useState} from 'react'
import './Register.scss'
import {NavLink, useNavigate} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import {Context} from "../../index";
import {observer} from "mobx-react-lite";

const Login = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [errors, setErrors] = useState<{[key: string]: string}>({});
    const [serverError, setServerError] = useState("");
    
    const {store} = useContext(Context);
    const navigate = useNavigate();
    
    // Валидация email
    const validateEmail = (email: string): boolean => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    };
    
    // Валидация формы
    const validateForm = (): boolean => {
        const newErrors: {[key: string]: string} = {};
        
        // Валидация почты
        if (!email.trim()) {
            newErrors.email = "Поле обязательно для заполнения";
        } else if (!validateEmail(email)) {
            newErrors.email = "Введите корректную почту";
        }
        
        // Валидация пароля
        if (!password) {
            newErrors.password = "Введите пароль";
        } else if (password.length < 6) {
            newErrors.password = "Неверный формат пароля";
        }
        
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };
    
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setServerError("");
        
        if (!validateForm()) {
            return;
        }
        
        setIsLoading(true);
        
        try {
            const result = await store.login(email, password);
            
            if (result.success) {
                navigate('/');
            } else {
                // Обработка ошибок от сервера
                let errorMessage = "";
                switch (result.errorType) {
                    case 'invalid_credentials':
                        errorMessage = "Неверная почта или пароль";
                        break;
                    case 'user_not_found':
                        errorMessage = "Такой пользователь не зарегистрирован";
                        break;
                    case 'server_error':
                    default:
                        errorMessage = "Ошибка сервера. Попробуйте позже.";
                        break;
                }
                setServerError(errorMessage);
                alert(errorMessage);
            }
        } catch (error) {
            const errorMessage = "Ошибка сервера. Попробуйте позже.";
            setServerError(errorMessage);
            alert(errorMessage);
        } finally {
            setIsLoading(false);
        }
    };
    
    return (
        <div className="register-page">
            {isLoading && <Waiter />}
            <form onSubmit={handleSubmit} className="login-form">
                <h2>Войти</h2>
                
                {serverError && (
                    <div className="server-error">
                        {serverError}
                    </div>
                )}
                
                <div className={`form-group ${errors.email ? 'error-form' : ''}`}>
                    <label htmlFor="email">Почта</label>
                    <input
                        type="text"
                        id="email"
                        name="email"
                        onChange={e => setEmail(e.target.value)}
                        value={email}
                        className={errors.email ? 'error-input' : ''}
                        disabled={isLoading}
                    />
                    {errors.email && <span className="error-message">{errors.email}</span>}
                </div>
                
                <div className={`form-group ${errors.password ? 'error-form' : ''}`}>
                    <label htmlFor="password">Пароль</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        onChange={e => setPassword(e.target.value)}
                        value={password}
                        className={errors.password ? 'error-input' : ''}
                        disabled={isLoading}
                    />
                    {errors.password && <span className="error-message">{errors.password}</span>}
                </div>
                
                <div className="forgot-password-link">
                    <NavLink to="/password-reset">Забыли пароль?</NavLink>
                </div>
                
                <button 
                    type="submit" 
                    className="login-button"
                    disabled={isLoading}
                >
                    {isLoading ? 'Загрузка...' : 'Войти'}
                </button>
                
                <NavLink to="/register">Регистрация</NavLink>
            </form>
        </div>
    )
}

export default observer(Login)