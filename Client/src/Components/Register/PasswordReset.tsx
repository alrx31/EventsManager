import React, {useState} from 'react';
import './Register.scss';
import {NavLink} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import AuthService from "../../services/AuthService";

const PasswordReset: React.FC = () => {
    const [email, setEmail] = useState("");
    const [isSubmitted, setIsSubmitted] = useState(false);
    const [error, setError] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [serverMessage, setServerMessage] = useState("");
    
    const validateEmail = (email: string): boolean => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    };
    
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        
        if (!email.trim()) {
            setError("Поле обязательно для заполнения");
            return;
        }
        
        if (!validateEmail(email)) {
            setError("Введите корректную почту");
            return;
        }
        
        setIsLoading(true);
        
        try {
            const response = await AuthService.resetPassword(email);
            
            if (response.data.success) {
                setServerMessage(response.data.message);
                setIsSubmitted(true);
            } else {
                setError(response.data.message || "Произошла ошибка. Попробуйте позже.");
            }
        } catch (err: any) {
            console.log('Password reset error:', err.response?.status, err.response?.data);
            setError("Произошла ошибка. Попробуйте позже.");
        } finally {
            setIsLoading(false);
        }
    };
    
    if (isSubmitted) {
        return (
            <div className="register-page">
                <div className="login-form">
                    <h2>Проверьте почту</h2>
                    <div className="success-message">
                        <p>{serverMessage || "Если аккаунт с такой почтой существует, новый пароль будет отправлен на указанный email."}</p>
                    </div>
                    <p style={{textAlign: 'center', marginBottom: '20px', color: 'var(--darkgray)', fontSize: '14px'}}>
                        Новый пароль был отправлен на <strong>{email}</strong>
                    </p>
                    <NavLink to="/login" className="back-to-login" style={{display: 'block', textAlign: 'center'}}>
                        Вернуться к входу
                    </NavLink>
                </div>
            </div>
        );
    }
    
    return (
        <div className="register-page">
            {isLoading && <Waiter />}
            <form onSubmit={handleSubmit} className="login-form">
                <h2>Восстановление пароля</h2>
                
                <p style={{textAlign: 'center', marginBottom: '20px', color: 'var(--darkgray)'}}>
                    Введите вашу почту и мы отправим новый пароль.
                </p>
                
                {error && (
                    <div className="server-error">
                        {error}
                    </div>
                )}
                
                <div className={`form-group ${error ? 'error-form' : ''}`}>
                    <label htmlFor="email">Почта</label>
                    <input
                        type="text"
                        id="email"
                        name="email"
                        onChange={e => setEmail(e.target.value)}
                        value={email}
                        className={error ? 'error-input' : ''}
                        disabled={isLoading}
                    />
                </div>
                
                <button 
                    type="submit" 
                    className="login-button"
                    disabled={isLoading}
                >
                    {isLoading ? 'Отправка...' : 'Отправить новый пароль'}
                </button>
                
                <NavLink to="/login">Вернуться к входу</NavLink>
            </form>
        </div>
    );
};

export default PasswordReset;
