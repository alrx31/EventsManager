import React, {useContext, useState} from 'react';
import "./Register.scss"
import {Context} from "../../index";
import {NavLink, useNavigate} from "react-router-dom";
import {observer} from "mobx-react-lite";
import {Waiter} from "../Waiter/Waiter";

const Register = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [repeatPassword, setRepeatPassword] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [errors, setErrors] = useState<{[key: string]: string}>({});
    const [serverError, setServerError] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    
    const {store} = useContext(Context);
    const navigate = useNavigate();
    
    // Валидация email
    const validateEmail = (email: string): boolean => {
        const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return emailRegex.test(email);
    };
    
    // Валидация имени/фамилии - только буквы, минимум 2 символа
    const validateName = (name: string): boolean => {
        const nameRegex = /^[a-zA-Zа-яА-ЯёЁ]{2,}$/;
        return nameRegex.test(name.trim());
    };
    
    const validateForm = (): boolean => {
        const newErrors: {[key: string]: string} = {};
        
        // Email validation
        if (!email.trim()) {
            newErrors.email = "Введите email";
        } else if (!validateEmail(email)) {
            newErrors.email = "Неверный формат email";
        }
        
        // Password validation
        if (!password) {
            newErrors.password = "Введите пароль";
        } else if (password.length < 6) {
            newErrors.password = "Пароль должен содержать минимум 6 символов";
        }
        
        // Repeat password validation
        if (!repeatPassword) {
            newErrors.repeatPassword = "Повторите пароль";
        } else if (password !== repeatPassword) {
            newErrors.repeatPassword = "Пароли не совпадают";
        }
        
        // First name validation
        if (!firstName.trim()) {
            newErrors.firstName = "Введите имя";
        } else if (!validateName(firstName)) {
            newErrors.firstName = "Имя должно содержать только буквы (минимум 2 символа)";
        }
        
        // Last name validation
        if (!lastName.trim()) {
            newErrors.lastName = "Введите фамилию";
        } else if (!validateName(lastName)) {
            newErrors.lastName = "Фамилия должна содержать только буквы (минимум 2 символа)";
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
            const result = await store.registration(email, password, firstName, lastName);
            
            if (result.success) {
                alert("Аккаунт успешно создан");
                navigate('/login');
            } else {
                // Обработка ошибок от сервера
                let errorMessage = "";
                switch (result.errorType) {
                    case 'email_exists':
                        errorMessage = "Пользователь с таким email уже существует";
                        break;
                    case 'invalid_data':
                        errorMessage = "Некорректно заполнены данные";
                        break;
                    case 'server_error':
                    default:
                        errorMessage = "Произошла ошибка. Попробуйте позже.";
                        break;
                }
                setServerError(errorMessage);
                alert(errorMessage);
                // Очищаем пароли при ошибке для безопасности
                setPassword("");
                setRepeatPassword("");
            }
        } catch (error) {
            const errorMessage = "Произошла ошибка. Попробуйте позже.";
            setServerError(errorMessage);
            alert(errorMessage);
        } finally {
            setIsLoading(false);
        }
    };
    
    return (
        <div className="register-page">
            {isLoading && <Waiter />}
            <form onSubmit={handleSubmit} className="register-form">
                <h2>Регистрация</h2>
                
                {serverError && (
                    <div className="server-error">
                        {serverError}
                    </div>
                )}
                
                <div className={`form-group ${errors.email ? 'error-form' : ''}`}>
                    <label htmlFor="email">Email</label>
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
                
                <div className={`form-group ${errors.repeatPassword ? 'error-form' : ''}`}>
                    <label htmlFor="repeatPassword">Повторите пароль</label>
                    <input
                        type="password"
                        id="repeatPassword"
                        name="repeatPassword"
                        onChange={e => setRepeatPassword(e.target.value)}
                        value={repeatPassword}
                        className={errors.repeatPassword ? 'error-input' : ''}
                        disabled={isLoading}
                    />
                    {errors.repeatPassword && <span className="error-message">{errors.repeatPassword}</span>}
                </div>
                
                <div className={`form-group ${errors.firstName ? 'error-form' : ''}`}>
                    <label htmlFor="firstName">Имя</label>
                    <input
                        type="text"
                        id="firstName"
                        name="firstName"
                        onChange={e => setFirstName(e.target.value)}
                        value={firstName}
                        className={errors.firstName ? 'error-input' : ''}
                        disabled={isLoading}
                    />
                    {errors.firstName && <span className="error-message">{errors.firstName}</span>}
                </div>
                
                <div className={`form-group ${errors.lastName ? 'error-form' : ''}`}>
                    <label htmlFor="lastName">Фамилия</label>
                    <input
                        type="text"
                        id="lastName"
                        name="lastName"
                        onChange={e => setLastName(e.target.value)}
                        value={lastName}
                        className={errors.lastName ? 'error-input' : ''}
                        disabled={isLoading}
                    />
                    {errors.lastName && <span className="error-message">{errors.lastName}</span>}
                </div>
                
                <button 
                    type="submit" 
                    className="register-button"
                    disabled={isLoading}
                >
                    {isLoading ? 'Регистрация...' : 'Зарегистрироваться'}
                </button>
                <NavLink to="/login">Уже есть аккаунт?</NavLink>
            </form>
        </div>
    )
}

export default observer(Register);