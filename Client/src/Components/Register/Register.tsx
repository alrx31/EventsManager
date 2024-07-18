import react, {useContext, useState} from 'react';
import "./Register.scss"
import Store from "../../store/store";
import {Context} from "../../index";
import {NavLink, useNavigate} from "react-router-dom";
import {observer} from "mobx-react-lite";
import React from "react";
const Register = (
    
) => {
    
    const [email,setEmale] = useState("");
    const [password, setPassword] = useState("")
    const [birthDate,setBirthDate] = useState(new Date())
    const [firstName,setFirstName] = useState("");
    const [lastName,setLastName] = useState("");
    
    const {store} = useContext(Context)
    
    const history = useNavigate();
    
    let handleSubmit = (e:any)=>{
        e.preventDefault();
        store.registration(email,password,firstName,lastName,birthDate);
        if(!store.isAuht){
            history('/login')
        }
    }
    
    return (
        <div className="register-page">
            <form onSubmit={handleSubmit}>
                <h2>Регистрация</h2>
                <div className="form-group">
                    <label htmlFor="email">Email</label>
                    <input
                        type="text"
                        id="email"
                        name="email"
                        onChange={e=>setEmale(e.target.value)}
                        value={email}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="password">Пароль</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        onChange={e=>setPassword(e.target.value)}
                        value={password}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="firstName">Имя</label>
                    <input
                        type="text"
                        id="firstName"
                        name="firstName"
                        onChange={e=>setFirstName(e.target.value)}
                        value={firstName}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="lastName">Фамилия</label>
                    <input
                        type="text"
                        id="lastName"
                        name="lastName"
                        onChange={e=>setLastName(e.target.value)}
                        value={lastName}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="birthDate">Дата рождения</label>
                    <input
                        type="date"
                        id="birthDate"
                        name="birthDate"
                        onChange={e=>setBirthDate(new Date(e.target.value))}
                        value={birthDate.toString()}
                    />
                </div>
                <button type="submit" className="login-button">Зарегистрироваться</button>
                <NavLink to={"/login"}>Уже есть аккаунт?</NavLink>
            </form>
        </div>
    )
}

export default observer(Register);