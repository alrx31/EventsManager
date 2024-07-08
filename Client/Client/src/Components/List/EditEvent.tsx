import React from 'react';
import './TestMenu.scss';
import {useNavigate, useParams} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import { IEventCreate } from '../../models/Event';

interface ITestMenuProps {}

export const EditEventMenu: React.FC<ITestMenuProps> = () => {
    let { EventId } = useParams();
    let [Event, setEvent] = React.useState<IEventCreate>({} as IEventCreate);
    let [isLoad, setIsLoad] = React.useState(false);
    let history = useNavigate();

    React.useEffect(() => {
        setIsLoad(true);
        //getTest();
        setIsLoad(false);
    }, []);

    
    return (
        <div className="update-test-page">
            {isLoad ? <Waiter /> : ""}
            <h1>Меню редактирования мероприятия</h1>
            <h2>Нажните 2 раза ПКМ на поле которое хотите отредактировать</h2>
            <h2>Для сохранения нажмите <i>Enter</i>, или нажмите вне текстого поля</h2>
            
        </div>
    );
}
