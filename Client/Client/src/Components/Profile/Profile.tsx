import React, {useContext, useEffect, useState} from 'react';
import {useNavigate, useParams} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import {Context} from "../../index";
import UserService from "../../services/UserService";
import {IUser} from "../../models/User";
import "./Profile.scss";
import {IEvent} from "../../models/Event";
import EventsService from "../../services/EventsService";
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
                    <span>Дата рождения:</span>
                    <span>{user?.birthDate?.toString()}</span>
                </div>

                <div className="profile-info-row">
                    <span>Дата регистрации:</span>
                    <span>{user?.registrationDate?.toString()}</span>
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
                    <h2>Всего: {events.length}</h2>
                    <div className="events-list">
                        {events.map((event:IEvent)=>{
                            return (
                                <div className="event" key={event.id}
                                     onClick={()=>{
                                         history(`/event/${event.id}`)
                                     }}
                                >
                                    <div className="event-image">
                                        <img src={event.imageSrc} alt=""/>
                                    </div>
                                    <div className="event-info">
                                        <h3>{event.name}</h3>
                                        <p>{event.description}</p>
                                        <p>{event.date.toString()}</p>
                                        <p>{event.location}</p>
                                        <p>{event.category}</p>
                                        <p>{event.maxParticipants}</p>
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

