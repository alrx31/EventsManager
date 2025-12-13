import React, {useContext, useEffect} from 'react';
import './TestMenu.scss';
import {useNavigate, useParams} from "react-router-dom";
import { IEvent } from '../../models/Event';
import EventsService from '../../services/EventsService';
import {Waiter} from "../Waiter/Waiter";
import {Context} from "../../index";
import ParticipantService from "../../services/ParticipantService";
import {IUser} from "../../models/User";
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
    let [participants,setParticipants] = React.useState<IUser[]>([]);
    let history = useNavigate();
    let {store} = useContext(Context)
    let [isFull, setIsFull] = React.useState(false);
    
    useEffect(()=>{
        const loadEvent = async () =>{
            setIsLoad(true);
            try{
                const response = await EventsService.fetchEvent(Number(EventId));
                if (response.status === 200) {
                    setEvent(response.data);
                    await checkParticipant();
                } else {
                    throw 'Ошибка получения данных';
                }
            }catch(e:any){
                console.log(e.response?.data?.message);
            }finally {
                setIsLoad(false);
            }
        }
        loadEvent();
    },[EventId])
    
    useEffect(()=>{
        if(!EventId) return;
        fetchParticipants();
    },[EventId, Event?.maxParticipants])
    
    
    if(isLoad){
        return <Waiter/> 
    }
    let checkParticipant = async ()=>{
        try{
            const response = await EventsService.getEvetnsByUserId(store.user.id);
            if(response.status === 200){
                let events = response.data;
                let event = events.find((event:IEvent)=>event.id === Number(EventId));
                if(event !== undefined){
                    setIsParticipant(true);
                }else{
                    setIsParticipant(false);
                }
            }else{
                throw 'Ошибка получения данных';
            }
        }catch(e:any){
            console.log(e.response?.data?.message);
        }
    }

    const fetchParticipants = async ()=>{
        try{
            const response = await EventsService.getParticipants(Number(EventId));
            if(response.status === 200){
                const list = response.data as IUser[];
                setParticipants(list);
                if(Event?.maxParticipants){
                    setIsFull(list.length >= Event.maxParticipants);
                }else{
                    setIsFull(false);
                }
            }else{
                throw 'Ошибка получения данных';
            }
        }catch(e:any){
            console.log(e.response?.data?.message);
            if(!store.user.isAdmin){
                alert("Ошибка получения данных, на мероприятие записаться не получится");
            }
        }
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
                    alert(e?.response?.data?.message || "Ошибка записи на мероприятие")
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

                <div className="EventInfo__details">
                    <div className="info-row">
                        <span className="label">Название:</span>
                        <span className="value">{Event?.name}</span>
                    </div>
                    <div className="info-row">
                        <span className="label">Описание:</span>
                        <span className="value">{Event?.description}</span>
                    </div>
                    <div className="info-row">
                        <span className="label">Место проведения:</span>
                        <span className="value">{Event?.location}</span>
                    </div>
                    <div className="info-row">
                        <span className="label">Дата проведения:</span>
                        <span className="value">{Event?.date ? new Date(Event.date).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit' }) : ''}</span>
                    </div>
                    <div className="info-row">
                        <span className="label">Категория:</span>
                        <span className="value">{Event?.category}</span>
                    </div>
                    <div className="info-row">
                        <span className="label">Максимальное количество участников:</span>
                        <span className="value">{Event?.maxParticipants}</span>
                    </div>
                    {(isFull && <div className="no-seats">Мест нет</div>)}
                    
                    {store.user.isAdmin && (
                        <div className="participants-block">
                            <h3>Участники ({participants.length}{Event?.maxParticipants ? ` / ${Event.maxParticipants}` : ""})</h3>
                            {participants.length > 0 ? (
                                <ul>
                                    {participants.map((p)=>(
                                        <li key={p.id}>
                                            {p.firstName} {p.lastName} — {p.email}
                                        </li>
                                    ))}
                                </ul>
                            ):(
                                <p>Пока никто не записался</p>
                            )}
                        </div>
                    )}
                    

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
        </div>
    )
};
export default EventMenu;