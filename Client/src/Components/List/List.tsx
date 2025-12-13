import React, {useContext, useEffect, useState} from 'react';
import './List.scss';
import {useNavigate} from "react-router-dom";
import {Context} from "../../index";
import {IEvent} from "../../models/Event";
import {observer} from "mobx-react-lite";
import EventsService from "../../services/EventsService";
import {Waiter} from "../Waiter/Waiter";
import {formatLocalDateTime} from "../../utils/date";
interface ListProps {
}

const List: React.FC<ListProps> = (
    {
        
    }
    ) => {
    const history = useNavigate();
    const {store} = useContext(Context);
    // пагинация
    const [events, setEvents] = React.useState<IEvent[]>([]);
    const [countPages, setCountPages] = React.useState(1);
    const [page, setPage] = React.useState(1);
    
    const [isLoad,setIsLoad] = React.useState(false);
    const [connectionError, setConnectionError] = React.useState(false);
    // поиск
    const [search, setSearch] = React.useState('');
    const [date, setDate] = React.useState(new Date());
    const [pageS,setPageS] = React.useState(1);
    
    // фильтрация
    const [location,setLocation] = useState("");
    const [category,setCategory] = useState("")
    const [pageF, setPageF] = useState(1);
    
    const [isInFilter,setIsFilter] = useState(false);
    const [isInSearch, setInSearch] = useState(false);
    let getEvents = async () =>{
        setConnectionError(false);
        setIsLoad(true);
        try{
            await countEvetns();
            const response = await EventsService.fetchEvents(page,store.pageSize);
            if(response.status == 200){
                setEvents(response.data);
            }else{
                throw 'Ошибка получения данных';
            }
        }catch (e:any){
            console.log(e.response?.data?.message);
            setConnectionError(true);
        } finally {
            setIsLoad(false);
        }
    }


    

    useEffect(() => {
        getEvents();
    }, [page]);
    useEffect(() => {
        if(!isInSearch) return;
        searchF();
    }, [pageS]);
    useEffect(() => {
        if(!isInFilter) return;
        filterF();
    }, [pageF]);
    
    let countEvetns = async () =>{
        try{
            const response = await EventsService.getCountEvents();
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
        }catch(e){
            console.log(e);
        }
    }
    let searchF = async ()=>{
        setConnectionError(false);
        setIsLoad(true);
        try{
            const response = await EventsService.searchEvents(search, date, pageS, store.pageSize);
            if(response.status == 200){
                setEvents(response.data);
                await getSearchEventsCount();
            }else{
                throw 'Ошибка поиска';
            }
        }catch(e:any){
            console.log(e);
            setConnectionError(true);
        } finally {
            setIsLoad(false);
        }
    }
    
    let filterF = async () =>{
        setConnectionError(false);
        setIsLoad(true);
        try{
            const response = await EventsService.filterEvents(location,category,pageF,store.pageSize);
            if(response.status === 200){
                setEvents(response.data);
                await getFilterEventsCount();
            }else{
                throw 'Ошибка фильтрации'
            }
        }catch(e:any){
            console.log(e);
            setConnectionError(true);
        } finally {
            setIsLoad(false);
        }
    }
    
    
    let handleChangePage = (el:number)=>{
        if(isInSearch){
            setPageS(el);
            return;
        }
        if(isInFilter){
            setPageF(el);
            return;
        }
        setPage(el);
    }
    let HandleSubmitSearch = (e:any)=>{
        e.preventDefault();
        setInSearch(true);
        setIsFilter(false);
        setPageS(1);
        searchF();
    }
    let HandleSubmitFilter = (e:any)=>{
        e.preventDefault();
        setIsFilter(true);
        setInSearch(false);
        setPageF(1);
        filterF();
    }
    
    let getSearchEventsCount = async ()=>{
        try{
            const response = await EventsService.getCountEventsSearch(search,date);
            if(response.status === 200){
                if(response.data != 0){
                    setCountPages(Math.ceil(response.data/store.pageSize));
                }
                else{
                    setCountPages(1)
                }
            }
        }catch(e){
            console.log(e);
        }
    }
    
    let getFilterEventsCount = async () =>{
        try{
            const response = await EventsService.getFilterEventsCount(location,category);
            if(response.status === 200){
                if(response.data != 0){
                    setCountPages(Math.ceil(response.data/store.pageSize));
                }
                else{
                    setCountPages(1)
                }
            }
        }catch(e){
            console.log(e);
        }
    }
    
    
    return (
        <div className={"list-page"}>
            {isLoad && <Waiter/>}

            <div className="List-bar">
                {store.user.isAdmin && (
                    <button
                        className={"create-event"}
                        onClick={() => history('/create-event')}
                    >Создать мероприятие
                    </button>
                )}

                <form
                    className="search-form"
                    onSubmit={HandleSubmitSearch}
                    onReset={() => {
                        setSearch("");
                        setDate(new Date());
                    }}
                    
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
                    <button
                        type={"reset"}
                        className={"user-logout"}
                        onClick={() => {
                            setInSearch(false);
                            setPage(1);
                            getEvents();
                        }}
                    >Сбросить
                    </button>
                </form>


                <div>
                    <button
                        className={"primary"}
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

                </div>
            </div>

            <div className="filter-menu">
                <form
                    onSubmit={HandleSubmitFilter}
                    onReset={() => {
                        setLocation("");
                        setCategory("");
                    }
                    }
                >
                    <label>Место проведения</label>
                    <input
                        type="text"
                        value={location}
                        onChange={(e) => setLocation(e.target.value)}
                    />
                    <label>Категория</label>
                    <input
                        type={"text"}
                        value={category}
                        onChange={(e) => setCategory(e.target.value)}
                    />
                    <button
                        type={"submit"}
                        className={"primary"}
                    >Фильтровать
                    </button>
                    <button
                        type={"reset"}
                        className={"user-logout"}
                        onClick={() => {
                            setIsFilter(false);
                            setPage(1);
                            getEvents();
                        }}
                    >Сбросить
                    </button>
                </form>
            </div>
            

            <ul>
                {countPages <= 1 ? null : (
                    Array.from({length: countPages}, (_, i) => i + 1).map((el, index) => (
                        <li
                            key={index}
                            onClick={() => handleChangePage(el)}
                            className={isInSearch ? (pageS == el ? 'active' : '') : (isInFilter ? (pageF == el ? 'active' : '') : (page == el ? 'active' : ''))}
                        >
                            {el}
                        </li>
                    ))
                )}
            </ul>

            <div className="list-table">
                {connectionError ? (
                    <div className="connection-error">
                        <h2>Ошибка подключения. Попробуйте позже.</h2>
                    </div>
                ) : events.length > 0 ? (
                    <>
                        <div className="table-header">
                            <div>Фото</div>
                            <div>Название</div>
                            <div>Описание</div>
                            <div>Дата</div>
                            <div>Место / Категория</div>
                            <div>Макс. мест</div>
                        </div>
                        {events.map((event, index) => (
                            <div key={index} className="table-row"
                                 onClick={() => history(`/event/${event.id}`)}
                            >
                                <div className="thumb">
                                    <img src={event.imageSrc} alt=""/>
                                </div>
                                <div className="title">{event.name}</div>
                                <div className="desc">{event.description}</div>
                                <div className="date">{event?.date ? formatLocalDateTime(event.date) : ''}</div>
                                <div className="meta">
                                    <span>{event.location}</span>
                                    <span>{event.category}</span>
                                </div>
                                <div className="max">{event.maxParticipants}</div>
                            </div>
                        ))}
                    </>
                ) : (
                    <h1>Мероприятий нет</h1>
                )}
            </div>

        </div>
    );
};
export default observer(List);