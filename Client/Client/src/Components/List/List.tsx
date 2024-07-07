import React, {useContext, useEffect, useState} from 'react';
import './List.scss';
import {useNavigate} from "react-router-dom";
import {Context} from "../../index";
import {IEvent} from "../../models/Event";
import {observer} from "mobx-react-lite";
import EventsService from "../../services/EventsService";
import {Waiter} from "../Waiter/Waiter";
interface ListProps {
}

const List: React.FC<ListProps> = (
    {
        
    }
    ) => {
    const history = useNavigate();
    const {store} = useContext(Context);
    // пагинация
    const [page, setPage] = React.useState(1);
    const [events, setEvents] = React.useState<IEvent[]>([]);
    const [countPages, setCountPages] = React.useState(1);
    
    const [isLoad,setIsLoad] = React.useState(false);
    
    const [search, setSearch] = React.useState('');
    const [date, setDate] = React.useState(new Date());
    const [pageS,setPageS] = React.useState(1);
    const [isInSearch, setInSearch] = useState(false);
    let getEvents = async () =>{
        try{
            await EventsService.fetchEvents(page,store.pageSize)
                .then((response)=>{
                    if(response.status == 200){
                        setEvents(response.data);
                    }else{
                        throw 'Ошибка получения данных';
                    }
                })
        }catch (e:any){
            console.log(e.response?.data?.message);
        }
    }


    

    useEffect(() => {
        setIsLoad(true)
        countEvetns();
        getEvents();
        setIsLoad(false)
    }, [page]);
    useEffect(() => {
        setIsLoad(true);
        searchF();
        setIsLoad(false)
    }, [pageS]);
    
    let countEvetns = () =>{
        EventsService.getCountEvents().then((response)=>{
            if(response.status == 200){
                if(response.data != 0){
                    setCountPages(Math.ceil(response.data/store.pageSize));
                }
                else{
                    setCountPages(1)
                }
            }else{
                throw 'Ошибка получения количества мероприятий';
            }
        }).catch(e=>console.log(e))
    }
    let searchF =async  ()=>{
        EventsService.searchEvents(search, date, pageS, store.pageSize).then((response)=>{
            if(response.status == 200){
                setEvents(response.data);
                if (events.length === 0) return;
                getSearchEventsCount()
            }else{
                throw 'Ошибка поиска';
            }    
        }).catch(e=>console.log(e));
    }
    
    let handleChangePage = (el:number)=>{
        if(isInSearch){
            setPageS(el)
        }else{
            setPage(el);
        }
    }
    let HandleSubmitSearch = (e:any)=>{
        e.preventDefault();
        setInSearch(true)
        setIsLoad(true)
        searchF();
        setIsLoad(false)
        
    }
    
    let getSearchEventsCount = ()=>{
        EventsService.getCountEventsSearch(search,date).then((response)=>{
            if(response.status === 200){
                if(response.data != 0){
                    setCountPages(Math.ceil(response.data/store.pageSize));
                }
                else{
                    setCountPages(1)
                }
            }
        }).catch((e=>console.log(e)))
        
    }
    
    
    return (
        <div className={"list-page"}>
            {isLoad && <Waiter/>}

            <div className="List-bar">
                <button
                    className={"create-event"}
                    onClick={() => history('/create-event')}
                >Создать мероприятие
                </button>

                <form
                    className="search-form"
                    onSubmit={HandleSubmitSearch}
                >
                    <div className="form-group">
                        <label>По имени</label>
                        <input
                            type="text"
                            placeholder="Поиск"
                            onChange={(e) => setSearch(e.target.value)}
                            value={search}
                        />
                    </div>
                    <div className="form-group">
                        <label>По дате</label>
                        <input
                            type="datetime-local"
                            id="dateTime"
                            placeholder={"Дата и время проведения"}
                            onChange={(e) => setDate(new Date(e.target.value))}
                        />
                    </div>
                    <button type="submit">Искать</button>
                </form>


                <div>
                    <button
                        className={"create-event"}
                        onClick={() => {
                            history(`/user/${store.user.id}`)
                        }}
                    >Профиль
                    </button>
                    <button
                        className={"user-logout"}
                        onClick={() => {
                            store.logout();
                            history('/login')
                        }}
                    >Выйти
                    </button>
                    <button
                        className={"user-logout"}
                        onClick={() => {
                            setInSearch(false);
                            history("/");
                            setPage(1);
                        }}
                    >Сбросить</button>
                </div>
            </div>

            <ul>
                {countPages > 1 ? (
                    Array.from({length: countPages}, (_, i) => i + 1).map((el, index) => (
                        <li
                            key={index}
                            onClick={() => handleChangePage(el)}
                            className={isInSearch ? (pageS == el ? 'active' : ''):(page == el ? 'active' : '')}
                        >
                            {el}
                        </li>
                    ))
                ) : (
                    null
                )}
            </ul>
            
            <div className="list">

                {events.length > 0 ?
                    events.map((event, index) => (
                        <div key={index} className="list-item"
                             onClick={() => history(`/event/${event.id}`)}
                        >
                            <p>{event.id}</p>
                            <div className="list-item__image">
                                <img src={event.imageSrc} alt=""/>
                            </div>
                            <div className="list-item__info">
                                <div className="list-item__info__name">
                                    Название:<p>{event.name}</p>
                                </div>
                                <div className="list-item__info__description">
                                    Описание:<p>{event.description}</p>
                                </div>
                                <div className="list-item__info__location">
                                    Локация:<p>{event.location}</p>
                                </div>
                                <div className="list-item__info__date">
                                    Дата:<p>{event?.date?.toString()}</p>
                                </div>
                                <div className="list-item__info__category">
                                    Категория:<p>{event.category}</p>
                                </div>
                                <div className="list-item__info__maxParticipants">
                                    Максимум учасников:<p>{event.maxParticipants}</p>
                                </div>
                            </div>
                        </div>
                    )) : (
                        <h1>Мероприятий нет</h1>
                    )

                }
                
                {//events.length > 0 && <button onClick={getEvents}>Загрузить еще</button>}
                }
            </div>

        </div>
    );
};
export default observer(List);