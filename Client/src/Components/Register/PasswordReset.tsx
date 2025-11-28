import React, {useState} from 'react';
import './Register.scss';
import {NavLink} from "react-router-dom";

const PasswordReset: React.FC = () => {
    const [email, setEmail] = useState("");
    const [isSubmitted, setIsSubmitted] = useState(false);
    const [error, setError] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    
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
        
        // TODO: Реализовать API вызов для восстановления пароля
        // Пока просто показываем сообщение об успехе
        setTimeout(() => {
            setIsSubmitted(true);
            setIsLoading(false);
        }, 1000);
    };
    
    if (isSubmitted) {
        return (
            <div className="register-page">
                <div className="login-form">
                    <h2>Проверьте почту</h2>
                    <p style={{textAlign: 'center', marginBottom: '20px', color: 'var(--darkgray)'}}>
                        Если аккаунт с почтой <strong>{email}</strong> существует, 
                        мы отправили инструкции по восстановлению пароля.
                    </p>
                    <NavLink to="/login" className="back-to-login">Вернуться к входу</NavLink>
                </div>
            </div>
        );
    }
    
    return (
        <div className="register-page">
            <form onSubmit={handleSubmit} className="login-form">
                <h2>Восстановление пароля</h2>
                
                <p style={{textAlign: 'center', marginBottom: '20px', color: 'var(--darkgray)'}}>
                    Введите вашу почту и мы отправим инструкции по восстановлению пароля.
                </p>
                
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
                    {error && <span className="error-message">{error}</span>}
                </div>
                
                <button 
                    type="submit" 
                    className="login-button"
                    disabled={isLoading}
                >
                    {isLoading ? 'Отправка...' : 'Отправить'}
                </button>
                
                <NavLink to="/login">Вернуться к входу</NavLink>
            </form>
        </div>
    );
};

export default PasswordReset;
