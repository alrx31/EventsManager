import react, {useContext, useState} from 'react';
import "./Register.scss"
import Store from "../../store/store";
import {Context} from "../../index";
import {NavLink, useNavigate} from "react-router-dom";
import {observer} from "mobx-react-lite";
import React from "react";

const Register = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [repeatPassword, setRepeatPassword] = useState("");
    const [birthDate, setBirthDate] = useState("");
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [errors, setErrors] = useState<{[key: string]: string}>({});
    
    const {store} = useContext(Context);
    const history = useNavigate();
    
    const validateEmail = (email: string): boolean => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    };
    
    const validateAge = (dateString: string): boolean => {
        if (!dateString) return false;
        const birthDateObj = new Date(dateString);
        const today = new Date();
        const age = today.getFullYear() - birthDateObj.getFullYear();
        const monthDiff = today.getMonth() - birthDateObj.getMonth();
        
        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDateObj.getDate())) {
            return age - 1 >= 14;
        }
        return age >= 14;
    };
    
    const validateForm = (): boolean => {
        const newErrors: {[key: string]: string} = {};
        
        // Email validation
        if (!email.trim()) {
            newErrors.email = "Email обязателен";
        } else if (!validateEmail(email)) {
            newErrors.email = "Неверный формат email";
        }
        
        // Password validation
        if (!password) {
            newErrors.password = "Пароль обязателен";
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
            newErrors.firstName = "Имя обязательно";
        }
        
        // Last name validation
        if (!lastName.trim()) {
            newErrors.lastName = "Фамилия обязательна";
        }
        
        // Birth date validation
        if (!birthDate) {
            newErrors.birthDate = "Дата рождения обязательна";
        } else if (!validateAge(birthDate)) {
            newErrors.birthDate = "Вам должно быть минимум 14 лет";
        }
        
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };
    
    const handleSubmit = async (e: any) => {
        e.preventDefault();
        
        if (!validateForm()) {
            return;
        }
        
        await store.registration(email, password, firstName, lastName, new Date(birthDate));
        if (!store.isAuht) {
            history('/login');
        }
    };
    
    return (
        <div className="register-page">
            <form onSubmit={handleSubmit} className="register-form">
                <h2>Регистрация</h2>
                
                <div className={`form-group ${errors.email ? 'error-form' : ''}`}>
                    <label htmlFor="email">Email</label>
                    <input
                        type="text"
                        id="email"
                        name="email"
                        onChange={e => setEmail(e.target.value)}
                        value={email}
                        className={errors.email ? 'error-input' : ''}
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
                    />
                    {errors.lastName && <span className="error-message">{errors.lastName}</span>}
                </div>
                
                <div className={`form-group ${errors.birthDate ? 'error-form' : ''}`}>
                    <label htmlFor="birthDate">Дата рождения</label>
                    <input
                        type="date"
                        id="birthDate"
                        name="birthDate"
                        onChange={e => setBirthDate(e.target.value)}
                        value={birthDate}
                        className={errors.birthDate ? 'error-input' : ''}
                    />
                    {errors.birthDate && <span className="error-message">{errors.birthDate}</span>}
                </div>
                
                <button type="submit" className="register-button">Зарегистрироваться</button>
                <NavLink to={"/login"}>Уже есть аккаунт?</NavLink>
            </form>
        </div>
    )
}

export default observer(Register);