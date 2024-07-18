import React, {useContext, useEffect} from 'react';
import './TestMenu.scss';
import {Await, useNavigate, useParams} from "react-router-dom";
import { IEvent } from '../../models/Event';
import EventsService from '../../services/EventsService';
import {Waiter} from "../Waiter/Waiter";
import {Context} from "../../index";
import ParticipantService from "../../services/ParticipantService";
interface IEventMenuProps {
}
const EventMenu:React.FC<IEventMenuProps> = (
    {
    }
) => {
    
    
    let {EventId} = useParams();
    let [Event,setEvent] = React.useState<IEvent>({} as IEvent);
    let [isLoad,setIsLoad] = React.useState(false);
    let [isParticipant,setIsParticipant] = React.useState(false);
    let history = useNavigate();
    let {store} = useContext(Context)
    let [isFull, setIsFull] = React.useState(true);
    
    useEffect(()=>{
        setIsLoad(true);
        
        EventsService.fetchEvent(Number(EventId))
            .then((response) => {
                if (response.status === 200) {
                    setEvent(response.data);
                    
                    
                    checkParticipant();
                } else {
                    throw 'Ошибка получения данных';
                }
            }).catch((e: any) => {
                console.log(e.response?.data?.message);
            })

        setIsLoad(false)
    },[EventId])
    
    useEffect(()=>{
        EventsService.getParticipants(Number(EventId))
            .then((response)=>{
                if(response.status === 200){
                    let participants = response.data;
                    if(Event?.maxParticipants && participants.length < Event?.maxParticipants){
                        setIsFull(false);
                    }
                }
            }).catch((e:any)=>{
            console.log(e.response?.data?.message);
            alert("Ошибка получения данных, на мероприятие записаться не получится");
        })
    },[Event])
    
    
    if(isLoad){
        return <Waiter/> 
    }
    let checkParticipant = ()=>{
        EventsService.getEvetnsByUserId(store.user.id)
            .then((response)=>{
                if(response.status === 200){
                    let events = response.data;
                    let event = events.find((event:IEvent)=>event.id === Number(EventId));
                    if(event !== undefined){
                        setIsParticipant(true);
                    }
                }else{
                    throw 'Ошибка получения данных';
                }
            }).catch((e:any)=>{
                console.log(e.response?.data?.message);
            })
    }
    
        
    let handleWrite = async ()=> {
        if (store.user !== null && Event !== null) {
            setIsLoad(true)
            await ParticipantService.CreteParticipant(Number(EventId), store.user.id)
                .then((response) => {
                    if (response.status === 200) {
                        alert("Вы успешно записались на мероприятие")
                        history("/");
                    } else {
                        throw "Ошибка записи на мероприятие"
                    }
                }).catch((e: any) => {
                    alert("Ошибка записи на мероприятие")
                    console.log(e.response?.data?.message)
            }).finally(()=>{
                setIsLoad(false)
            })
        }else{
            console.log("store.user или Event равны null", store.user, Event);
        }
    }
    let handleDelete =()=>{
        setIsLoad(true);
        EventsService.deleteEventParticipant(Number(EventId),store.user.id)
            .then((response)=>{
                if(response.status === 200){
                    alert("Вы успешно отписались от мероприятия");
                    history("/");
                }
                else{
                    throw "Ошибка отписки от мероприятия";
                }
            }).catch((e:any)=> {
            alert("Ошибка отписки от мероприятия");
            console.log(e.response?.data?.message);
        })
            .finally(()=>{
                setIsLoad(false);
            })
    }
    
    let getParticipants = ()=>{
        
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
                {(isFull && <h2>мест нет</h2>)}
                

                <div className="event-controll">
                    
                    {store.user.isAdmin && (<button
                        className={"edit-event"}
                        onClick={() => history(`/update/${Event?.id}`)}
                    >Редактировать</button>)}

                    {!store.user.isAdmin && (!isParticipant ? (

                        <button
                            className={"event-register"}
                            onClick={handleWrite}
                            disabled={isFull}
                        >Записаться </button>
                    ) : (
                        <button
                            className={"event-delete"}
                            onClick={handleDelete}
                        >Отписаться </button>
                    ))}

                    <button
                        className={"event-back"}
                        onClick={() => history("/")}
                    >Назад</button>
                </div>
                
            </div>
        </div>
    )
};
export default EventMenu;