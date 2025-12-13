import React, {useContext, useEffect, useState} from 'react';
import {useNavigate, useParams} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import {Context} from "../../index";
import UserService from "../../services/UserService";
import {IUser} from "../../models/User";
import "./Profile.scss";
import {IEvent} from "../../models/Event";
import EventsService from "../../services/EventsService";
import {formatLocalDateTime} from "../../utils/date";
interface IProfileProps {
}
export const Profile:React.FC<IProfileProps> = ({

})=> {
    let {UserId} = useParams();

    let [isLoad, setIsLoad] = React.useState(false);
    let history = useNavigate();
    let {store} = useContext(Context);
    let [user,setUser] = useState({} as IUser);
    let [events,setEvents] = useState<IEvent[]>([] as IEvent[]);
    
    
    useEffect(() => {
        setIsLoad(true)
        getUser(Number(UserId));
        getEvents(Number(UserId));
    }, []);
    
    if(isLoad){
        return <Waiter />
    }
    
    let getUser = (UserId:number)=>{
        try{
            UserService.fetchUserById(UserId)
                .then((response)=>{
                    if(response.status === 200){
                        setUser(response.data);
                    }
                }).catch((err)=>{
                console.log(err);
            })
                .finally(()=>{
                    setIsLoad(false);
                })
        }catch(err){
            console.log(err);
        }
    }
    
    let getEvents = async (UserId:number)=>{
        setIsLoad(true);
        try{
            await EventsService.getEvetnsByUserId(UserId)
                .then((response)=>{
                        if(response.status === 200){
                        setEvents(response.data);
                    }else{
                        throw 'Ошибка получения данных';
                }
                }).catch((err)=> {
                    console.log(err);
                })
        }catch(err:any){
            console.log(err);
        }finally {
            setIsLoad(false);
        }
    }
    
    return (
        <div className={"profile-page"}>
            {isLoad && <Waiter />}
                <h2>Профиль пользователя </h2>


            <div className="profile-info">
                <div className="profile-info-row">
                    <span>Имя:</span>
                    <span>{user.firstName}</span>
                </div>

                <div className="profile-info-row">
                    <span>Дата регистрации:</span>
                    <span>{user?.registrationDate ? formatLocalDateTime(user.registrationDate) : ''}</span>
                </div>

                <div className="profile-info-row">
                    <span>Фамилия:</span>
                    <span>{user.lastName}</span>
                </div>
                <div className="profile-info-row">
                    <span>Почта:</span>
                    <span>{user.email}</span>
                </div>

            </div>

            {!store.user.isAdmin ? (
                <div className="events">
                    <h2>Мероприятия пользователя</h2>
                    <h3>Всего: {events.length}</h3>
                    <div className="events-list">
                        <div className="events-header">
                            <div>Фото</div>
                            <div>Название</div>
                            <div>Описание</div>
                            <div>Дата</div>
                            <div>Место / Категория</div>
                        </div>
                        {events.map((event:IEvent)=>{
                            return (
                                <div className="event-row" key={event.id}
                                     onClick={()=>{
                                         history(`/event/${event.id}`)
                                     }}
                                >
                                    <div className="event-thumb">
                                        <img src={event.imageSrc} alt=""/>
                                    </div>
                                    <div className="event-title">{event.name}</div>
                                    <div className="event-desc">{event.description}</div>
                                    <div className="event-date">{formatLocalDateTime(event.date)}</div>
                                    <div className="event-meta">
                                        <span>{event.location}</span>
                                        <span>{event.category}</span>
                                        <span>До {event.maxParticipants} мест</span>
                                    </div>
                                </div>
                            )
                        })}
                    </div>
                </div>
            ):(
                <h2>Вы админ</h2>
            ) }
            
            
            <div className="profile-controlls">
                <button
                    onClick={() => history(`/`)}
                >Назад
                </button>
            </div>
        </div>
    );
}

