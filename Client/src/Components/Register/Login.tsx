import React, {FormEventHandler, useContext, useState} from 'react'
import './Register.scss'
import {NavLink, useNavigate} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import {IUser} from "../../models/User";
import {Context} from "../../index";
import {observer} from "mobx-react-lite";

const Login = (
    {setUser = (data:IUser) => {}}:any
)=>{
    const [email,setEmale] = useState("");
    const [password, setPassword] = useState("")
    const [isLoad,setIsLoad] = useState(false)
    const {store} = useContext(Context);
    
    let handleSubmit = (e:any)=>{
        e.preventDefault();
        store.login(email,password);
    }
    return (
        <div className="register-page">
            {isLoad ? <Waiter/> : ""}
            <h2>Войти</h2>
            <form onSubmit={handleSubmit} className={"login-form"}>
                <div className="form-group">
                    <label htmlFor="login">Почта</label>
                    <input
                        type="text"
                        id="login"
                        name="login"
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
                <button type="submit" className="login-button">Войти</button>
                <NavLink to={'/register'}>Регистрация</NavLink>
            </form>
        </div>
    )
}

export default observer(Login)