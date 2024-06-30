import {AxiosResponse} from "axios";
import $api from "../http";
import {IEvent, IEventCreate} from "../models/Event";

export default  class EventsService{
    static fetchEvents(page:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent>>(`/Events/events/${page}`);
    }
    static fetchEvent(id: number): Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent>>(`/Events/event/${id}`);
    }
    static createEvent(event: IEventCreate): Promise<AxiosResponse> {
        return $api.post<AxiosResponse<IEventCreate>>('/Events/create', event);
    }
} 