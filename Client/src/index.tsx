import React, {createContext} from 'react';
import ReactDOM from 'react-dom/client';
import './index.scss';
import App from './App';
import { BrowserRouter } from 'react-router-dom';
import Store from "./store/store";

interface StoreState{
    store:Store
}


const store = new Store();

export const  Context = createContext<StoreState>({
    store
})

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
    
  <BrowserRouter>
    <Context.Provider value={{
        store
    }}>
      <App />
    </Context.Provider>
  </BrowserRouter>
);