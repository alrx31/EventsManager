import React from 'react';
import "./CreatePage.scss";
import {useNavigate} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import {IEventCreate} from "../../models/Event";
import EventsService from "../../services/EventsService";

interface CreatePageProps {
}

export const CreatePage:React.FC<CreatePageProps> = (
    {
        
    }
) => {
    let history = useNavigate();
    const [isLoad, setIsLoad] = React.useState(false);
    
    const [event, setevent] = React.useState<IEventCreate>({
    } as IEventCreate);

    
    let HandleCreateevent = (e:any) => {
        e.preventDefault();
        if(!event.name || !event.description || !event.location || !event.category || !event.maxParticipants || !event.imageData){
            alert("Заполните все поля");
            return;
        }
        
        
        
        setIsLoad(true);
        EventsService.createEvent(event)
            .then((response) => {
                if (response.status === 200) {
                    alert("Мероприятие создано")
                    history('/');
                } else {
                    throw 'Ошибка получения данных';
                }
            }).catch((e: any) => {
                console.log(e.response?.data?.message);
                alert("Ошибка создания мероприятия")
            }).finally(() => {
                setIsLoad(false);
            });
            
    }
    
    
    
        return (
        <div className="create-page">
            {isLoad && <Waiter />}
            <form
                className="create-page-wrapper"
                onSubmit={HandleCreateevent}
            >
                <h1>Меню Создания Мероприятия</h1>
                <div className="create-event">
                    <label htmlFor="name">Имя</label>
                    <input 
                        type="text" 
                        id="name"
                        placeholder={"Имя"}
                        onChange={(e) => setevent({...event,name: e.target.value})}
                    />
                    <label htmlFor="description">Описание</label>
                    <textarea id="description" placeholder={"Описание"}
                                onChange={(e) => setevent({...event, description: e.target.value})}
                    />
                    <label htmlFor="location">Место проведения</label>
                    <input 
                        type="text" 
                        id="location"
                        placeholder={"Место проведения"}
                        onChange={(e) => setevent({...event, location: e.target.value})}
                    />
                    <label htmlFor="dateTime">Дата и время проведения</label>
                    <input
                        type="datetime-local"
                        id="dateTime"
                        placeholder={"Дата и время проведения"}
                        onChange={(e) => setevent({...event, date: new Date(e.target.value)})}
                        value={event.date?.toISOString().slice(0, 16)}
                    />
                    
                    
                    <label htmlFor="category">Категория</label>
                    <input 
                        type="text" 
                        id="category"
                        placeholder={"Категория"}
                        onChange={(e) => setevent({...event, category: e.target.value})}
                    />
                    <label htmlFor="maxParticipants">Максимальное количество участников</label>
                    <input 
                        type="number" 
                        id="maxParticipants"
                        placeholder={"Максимальное количество участников"}
                        onChange={(e) => setevent({...event, maxParticipants: Number(e.target.value)})}
                    />
                    <label htmlFor="imageSrc">Изображение</label>
                    <input
                        type={"file"}
                        id="imageData"
                        placeholder={"Изображение"}
                        onChange={(e) => setevent({...event, imageData: e.target.files?.[0] as File})}
                    />
                    

                    

                </div>
                <div className="create-controll">
                    <button onClick={() => history("/")}>Отмена</button>
                    <button type={"submit"}>Создать</button>
                </div>
            </form>

            
            
        </div>
    );
};
