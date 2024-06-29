import {AxiosResponse} from "axios";
import $api from "../http";
import {IEvent} from "../models/Event";

export default  class EventsService{
    static fetchEvents(page:number):Promise<AxiosResponse>{
        return $api.get<AxiosResponse<IEvent>>(`/Events/events/${page}`);
    }
} 