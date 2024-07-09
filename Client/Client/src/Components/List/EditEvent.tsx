import React from 'react';
import './TestMenu.scss';
import {useNavigate, useParams} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import { IEventCreate } from '../../models/Event';
import EventsService from "../../services/EventsService";

interface ITestMenuProps {}

export const EditEventMenu: React.FC<ITestMenuProps> = () => {
    let { EventId } = useParams();
    let [Event, setEvent] = React.useState<IEventCreate>({} as IEventCreate);
    let [isLoad, setIsLoad] = React.useState(false);
    let history = useNavigate();
    
    let [eventUptd, setEventUptd] = React.useState<IEventCreate>({} as IEventCreate);
    

    React.useEffect(() => {
        setIsLoad(true);
        getEvent();
        setIsLoad(false);
    }, []);

    let getEvent = ()=>{
        EventsService.fetchEvent(Number(EventId)).then((response) => {
            if (response.status === 200) {
                setEventUptd({
                    name: response.data.name,
                    description: response.data.description,
                    date: response.data.date,
                    location: response.data.location,
                    category: response.data.category,
                    maxParticipants: response.data.maxParticipants,
                    imageData: response.data.imageData
                });
            } else {
                throw 'Ошибка получения данных';
            }
        }
        ).catch((e: any) => {
            console.log(e.response?.data?.message);
        });
    }
    
    return (
        <div className="update-test-page">
            {isLoad ? <Waiter /> : ""}
            <h1>Меню редактирования мероприятия</h1>
            <h2>введите значение если хотите изменить его</h2>
        </div>
    );
}
