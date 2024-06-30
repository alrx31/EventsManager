import React, {useContext, useEffect, useState} from 'react';
import './App.scss';
import {Route, Routes, useNavigate} from 'react-router-dom'
import Login from "./Components/Register/Login";
import Register from "./Components/Register/Register";
import {Context} from "./index";
import {observer} from "mobx-react-lite";
import {Waiter} from "./Components/Waiter/Waiter";
import List from "./Components/List/List";
import EventMenu from './Components/List/EventMenu';
import {CreatePage} from "./Components/List/CreatePage";

function App() {
    let history = useNavigate();

    const {store} = useContext(Context)
    
    
    useEffect(() => {
        if(localStorage.getItem('token')){
            store.checkAuth()
        }
    }, []);


    useEffect(() => {
        if(!store.isAuht) {
            history('/login')
        }else{
            history('/')
        }
    }, [store.isAuht]);
    
    if(store.isLoading){
        return <Waiter/>
    }


    
    
    
  return (
      
    <div className="App">
        
    
      <Routes>
         <Route path={"/login"}  element={<Login/>} />
              <Route path={"/"} element={<List />} />
              <Route path={"/register"} element={<Register />} />
              <Route path={"/event/:EventId"} element={<EventMenu />} />
              <Route path={"/create-event"} element={<CreatePage />} />
      </Routes>
    </div>
  );
}

export default observer(App);