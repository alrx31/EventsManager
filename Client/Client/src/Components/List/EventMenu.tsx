import React from 'react';
import './TestMenu.scss';
import {useNavigate, useParams} from "react-router-dom";
import { IEvent } from '../../models/Event';
import EventsService from '../../services/EventsService';
import {Waiter} from "../Waiter/Waiter";
interface IEventMenuProps {
}
export const EventMenu:React.FC<IEventMenuProps> = (
    {
    }
) => {
    let {EventId} = useParams();
    let [Event,setEvent] = React.useState<IEvent|undefined>(undefined);
    let [isLoad,setIsLoad] = React.useState(false);
    let history = useNavigate();
    
    React.useEffect(()=>{
        setIsLoad(true);
        EventsService.fetchEvent(Number(EventId))
            .then((response) => {
                if (response.status === 200) {
                    setEvent(response.data);
                } else {
                    throw 'Ошибка получения данных';
                }
            }).catch((e: any) => {
                console.log(e.response?.data?.message);
            }).finally(() => {
                setIsLoad(false);
            });
    },[EventId])
    
if(isLoad){
        return <Waiter/> 
    }
        
    
    
    
    return (
        <div className={"EventMenu"}>
            <div className="EventInfo">
                <div className="EventInfo__image">
                    <img src={Event?.imageSrc} alt=""/>
                </div>

                <h2>Название: {Event?.name}</h2>
                <h2>Описание: {Event?.description}</h2>
                <h2>Место проведения: {Event?.location}</h2>
                <h2>Дата проведения: {Event?.date?.toString()}</h2>
                <h2>Категория: {Event?.category}</h2>
                <h2>Максимальное количество участников: {Event?.maxParticipants}</h2>


                <div className="event-controll">
                    
                    <button
                        className={"edit-event"}
                        onClick={() => history(`/update/${Event?.id}`)}
                    >Редактировать</button>
                    
                    <button
                        className={"event-back"}
                        onClick={() => history("/")}
                    >Назад</button>
                </div>
                
            </div>
        </div>
    );
};