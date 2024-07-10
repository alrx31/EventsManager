import React from 'react';
import './TestMenu.scss';
import {useNavigate, useParams} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import {IEvent, IEventCreate} from '../../models/Event';
import EventsService from "../../services/EventsService";

interface ITestMenuProps {}

export const EditEventMenu: React.FC<ITestMenuProps> = () => {
    let { EventId } = useParams();
    let [Event, setEvent] = React.useState<IEvent>({} as IEvent);
    let [isLoad, setIsLoad] = React.useState(false);
    let history = useNavigate();
    let [isModal,setIsModal] = React.useState(false);
        
    
    let [eventUptd, setEventUptd] = React.useState<IEventCreate>({
        name: "",
        description: "",
        date: new Date(),
        location: "",
        category: "",
        maxParticipants: 0,
        imageData: new File([""], "")
    } as IEventCreate);
    

    React.useEffect(() => {
        setIsLoad(true);
        getEvent();
        setIsLoad(false);
    }, []);

    let getEvent = ()=>{
        EventsService.fetchEvent(Number(EventId)).then((response) => {
            if (response.status === 200) {
                setEvent(response.data);
            } else {
                throw 'Ошибка получения данных';
            }
        }
        ).catch((e: any) => {
            console.log(e.response?.data?.message);
        });
    }
    
    let handleDeleteEvent = ()=>{
        setIsLoad(true)
        EventsService.deleteEvent(Number(EventId)).then((response) => {
            if(response.status === 200) {
                alert("Мероприятие удалено")
            } else {
                throw 'Ошибка удаления мероприятия';
            }
        }).catch(e=>{
            console.log(e.response?.data?.message);
            alert("Ошибка удаления мероприятия")
        }).finally(()=>{
            setIsLoad(false)
            history("/")
        })
    }
    
    let handleSubmitUpdatePage = ()=>{
        setIsLoad(true)
        EventsService.updateEvent(eventUptd,Number(EventId)).then((response) => {
            if(response.status === 200) {
                alert("Мероприятие обновлено")
            } else {
                throw 'Ошибка обновления мероприятия';
            }
        }).catch(e=>{
            console.log(e.response?.data?.message);
            alert("Ошибка обновления мероприятия")
        }).finally(()=>{
            setIsLoad(false)
            history("/")
        })
    }
    
    return (
        <div className="update-test-page">
            {isLoad ? <Waiter/> : ""}
            <h1>Меню редактирования мероприятия</h1>
            <h2>введите значение если хотите изменить его</h2>

            {(!isLoad && Event != null) && (
                <form
                    className="create-page-wrapper"
                    onSubmit={handleSubmitUpdatePage}
                >
                    <h1>Меню редактирования Мероприятия</h1>
                    <div className="create-eventUptd">
                        <label htmlFor="name">Названия</label>
                        <input
                            type="text"
                            id="name"
                            placeholder={Event.name}
                            onChange={(e) => setEventUptd({...eventUptd, name: e.target.value})}
                            value={eventUptd.name}
                        />
                        <label htmlFor="description">Описание</label>
                        <input id="description" placeholder={Event.description}
                                  onChange={(e) => setEventUptd({...eventUptd, description: e.target.value})}
                                    value={eventUptd.description}
                        />
                        <label htmlFor="location">Место проведения</label>
                        <input
                            type="text"
                            id="location"
                            placeholder={Event.location}
                            onChange={(e) => setEventUptd({...eventUptd, location: e.target.value})}
                            value={eventUptd.location}
                        />
                        <label htmlFor="dateTime">Дата и время проведения</label>
                        <input
                            type="datetime-local"
                            id="dateTime"
                            placeholder={"la"}
                            onChange={(e) => setEventUptd({...eventUptd, date: new Date(e.target.value)})}
                            value={eventUptd.date?.toISOString().slice(0, 16)}
                        />


                        <label htmlFor="category">Категория</label>
                        <input
                            type="text"
                            id="category"
                            placeholder={Event.category}
                            onChange={(e) => setEventUptd({...eventUptd, category: e.target.value})}
                            value={eventUptd.category}
                        />
                        <label htmlFor="maxParticipants">Максимальное количество участников</label>
                        <input
                            type="number"
                            id="maxParticipants"
                            placeholder={`${Event.maxParticipants}`}
                            onChange={(e) => setEventUptd({...eventUptd, maxParticipants: Number(e.target.value)})}
                            value={eventUptd.maxParticipants}
                        />
                        <label htmlFor="imageSrc">Изображение</label>
                        <img src={Event.imageSrc} alt=""/>
                        <input
                            type={"file"}
                            id="imageData"
                            placeholder={"Изображение"}
                            onChange={(e) => setEventUptd({...eventUptd, imageData: e.target.files?.[0] as File})}
                        />


                    </div>
                    <h2>Если Какие либо поля заполнены, то мероприятие будет обновлено</h2>
                    
                    <div className="create-controll">
                        <button type={"button"} onClick={() => history("/")}>Отмена</button>
                        
                        <button
                            type={"button"}
                            onClick={() => setIsModal(true)}
                        >Удалить<br/> мероприятие
                        </button>
                        
                        <button type={"submit"}>Обновить</button>
                    </div>
                </form>
            )}
            {// подтверждение удаления
                isModal && (
                    <div className="modal">
                        <div className="modal-content">
                            <h1>Вы уверены что хотите удалить мероприятие?</h1>
                            <div className="modal-controll">
                                <button onClick={() => setIsModal(false)}>Отмена</button>
                                <button onClick={handleDeleteEvent}>Удалить</button>
                            </div>
                        </div>
                    </div>
                )
            }
            
        </div>
    );
}
