import React, {useContext, useEffect} from 'react';
import './App.scss';
import {Route, Routes, useNavigate} from 'react-router-dom'
import Login from "./Components/Register/Login";
import Register from "./Components/Register/Register";
import {Context} from "./index";
import {observer} from "mobx-react-lite";
function App() {
    
    let history = useNavigate();

    const {store} = useContext(Context)

    useEffect(() => {
        if(localStorage.getItem('token')){
            store.checkAuth()
        }
    }, []);
    useEffect(() => {
        if(store.isAuht){
            history('/')
        }else{
            history('/login')
        }
    }, [store.isAuht]);
    
  return (
      
    <div className="App">
      
        <h1
            style={{
                position:"fixed",
                zIndex:1000
            }}
        >{store.isAuht ? "супер" : "мимо"}</h1>

      <Routes>
         <Route path={"/login"}  element={<Login/>} />
          <Route path={"/register"} element={<Register />}/>
      </Routes>
    </div>
  );
}

export default observer(App);
