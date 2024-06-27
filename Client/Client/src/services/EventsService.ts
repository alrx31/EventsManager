import {AxiosResponse} from "axios";
import $api from "../http";

export default  class UserService{
    static fetchEvents():Promise<AxiosResponse>{
        return $api.get('/api/events');
    }
} 