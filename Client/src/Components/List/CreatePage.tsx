import React, { useState } from 'react';
import "./CreatePage.scss";
import {useNavigate} from "react-router-dom";
import {Waiter} from "../Waiter/Waiter";
import {IEventCreate} from "../../models/Event";
import EventsService from "../../services/EventsService";

interface CreatePageProps {
}

interface ValidationErrors {
    name?: string;
    description?: string;
    location?: string;
    date?: string;
    category?: string;
    maxParticipants?: string;
    imageData?: string;
}

export const CreatePage:React.FC<CreatePageProps> = (
    {
        
    }
) => {
    let history = useNavigate();
    const [isLoad, setIsLoad] = React.useState(false);
    const [serverError, setServerError] = useState<string>('');
    const [errors, setErrors] = useState<ValidationErrors>({});
    
    const [event, setevent] = React.useState<IEventCreate>({
    } as IEventCreate);

    const validateForm = (): boolean => {
        const newErrors: ValidationErrors = {};
        
        // Валидация имени (обязательное, 3-50 символов)
        if (!event.name || event.name.trim() === '') {
            newErrors.name = 'Введите название мероприятия';
        } else if (event.name.length < 3) {
            newErrors.name = 'Название слишком короткое';
        } else if (event.name.length > 50) {
            newErrors.name = 'Название слишком длинное';
        }
        
        // Валидация описания (необязательное, но если заполнено: 10-200 символов)
        if (event.description && event.description.trim() !== '') {
            if (event.description.length < 10) {
                newErrors.description = 'Описание слишком короткое';
            } else if (event.description.length > 200) {
                newErrors.description = 'Описание слишком длинное';
            }
        }
        
        // Валидация места проведения (обязательное, 3-100 символов)
        if (!event.location || event.location.trim() === '') {
            newErrors.location = 'Укажите место проведения';
        } else if (event.location.length < 3) {
            newErrors.location = 'Описание слишком короткое';
        } else if (event.location.length > 100) {
            newErrors.location = 'Название слишком длинное';
        }
        
        // Валидация даты (обязательное, не в прошлом)
        if (!event.date) {
            newErrors.date = 'Укажите дату и время';
        } else {
            const selectedDate = new Date(event.date);
            const now = new Date();
            if (selectedDate < now) {
                newErrors.date = 'Дата не может быть в прошлом';
            }
        }
        
        // Валидация категории (необязательное, до 50 символов)
        if (event.category && event.category.length > 50) {
            newErrors.category = 'Название слишком длинное';
        }
        
        // Валидация максимального количества участников (необязательное, >= 1)
        if (event.maxParticipants !== undefined && event.maxParticipants !== 0) {
            if (event.maxParticipants < 1) {
                newErrors.maxParticipants = 'Введите число не меньше 1';
            }
        }
        
        // Валидация изображения (формат и размер)
        if (event.imageData) {
            const file = event.imageData;
            const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];
            const maxSize = 5 * 1024 * 1024; // 5 MB
            
            if (!allowedTypes.includes(file.type)) {
                newErrors.imageData = 'Допустимые форматы: .jpg, .png, .jpeg';
            } else if (file.size > maxSize) {
                newErrors.imageData = 'Максимальный размер файла: 5 МБ';
            }
        }
        
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleCancel = () => {
        // Проверяем, заполнены ли поля
        const hasData = event.name || event.description || event.location || 
                       event.date || event.category || event.maxParticipants || event.imageData;
        
        if (hasData) {
            if (window.confirm('Вы уверены, что хотите отменить? Все введённые данные будут потеряны.')) {
                history('/');
            }
        } else {
            history('/');
        }
    };
    
    let HandleCreateevent = (e:any) => {
        e.preventDefault();
        setServerError('');
        
        if (!validateForm()) {
            return;
        }
        
        setIsLoad(true);
        EventsService.createEvent(event)
            .then((response) => {
                if (response.status === 200) {
                    alert("Мероприятие создано");
                    history('/');
                } else {
                    throw new Error('Ошибка создания мероприятия');
                }
            }).catch((e: any) => {
                console.log(e.response?.data?.message);
                if (e.response?.status === 403) {
                    setServerError('Доступ запрещён');
                } else if (e.response?.status === 400) {
                    setServerError('Некорректные данные');
                } else if (e.response?.status === 500) {
                    setServerError('Ошибка сервера. Попробуйте позже.');
                } else {
                    setServerError('Ошибка создания мероприятия');
                }
            }).finally(() => {
                setIsLoad(false);
            });
            
    }
    
    // Форматирование даты для datetime-local input
    const formatDateForInput = (date: Date | undefined): string => {
        if (!date) return '';
        try {
            const d = new Date(date);
            const year = d.getFullYear();
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const day = String(d.getDate()).padStart(2, '0');
            const hours = String(d.getHours()).padStart(2, '0');
            const minutes = String(d.getMinutes()).padStart(2, '0');
            return `${year}-${month}-${day}T${hours}:${minutes}`;
        } catch {
            return '';
        }
    };
    
        return (
        <div className="create-page">
            {isLoad && <Waiter />}
            <form
                className="create-page-wrapper"
                onSubmit={HandleCreateevent}
            >
                <h1>Меню Создания Мероприятия</h1>
                
                {serverError && <div className="server-error">{serverError}</div>}
                
                <div className="create-event">
                    <label htmlFor="name">Имя</label>
                    <input 
                        type="text" 
                        id="name"
                        placeholder={"Имя"}
                        className={errors.name ? 'error' : ''}
                        onChange={(e) => setevent({...event,name: e.target.value})}
                    />
                    {errors.name && <span className="error-message">{errors.name}</span>}
                    
                    <label htmlFor="description">Описание</label>
                    <textarea 
                        id="description" 
                        placeholder={"Описание"}
                        className={errors.description ? 'error' : ''}
                        onChange={(e) => setevent({...event, description: e.target.value})}
                    />
                    {errors.description && <span className="error-message">{errors.description}</span>}
                    
                    <label htmlFor="location">Место проведения</label>
                    <input 
                        type="text" 
                        id="location"
                        placeholder={"Место проведения"}
                        className={errors.location ? 'error' : ''}
                        onChange={(e) => setevent({...event, location: e.target.value})}
                    />
                    {errors.location && <span className="error-message">{errors.location}</span>}
                    
                    <label htmlFor="dateTime">Дата и время проведения</label>
                    <input
                        type="datetime-local"
                        id="dateTime"
                        placeholder={"Дата и время проведения"}
                        className={errors.date ? 'error' : ''}
                        onChange={(e) => setevent({...event, date: new Date(e.target.value)})}
                        value={formatDateForInput(event.date)}
                    />
                    {errors.date && <span className="error-message">{errors.date}</span>}
                    
                    <label htmlFor="category">Категория</label>
                    <input 
                        type="text" 
                        id="category"
                        placeholder={"Категория"}
                        className={errors.category ? 'error' : ''}
                        onChange={(e) => setevent({...event, category: e.target.value})}
                    />
                    {errors.category && <span className="error-message">{errors.category}</span>}
                    
                    <label htmlFor="maxParticipants">Максимальное количество участников</label>
                    <input 
                        type="number" 
                        id="maxParticipants"
                        placeholder={"Максимальное количество участников"}
                        className={errors.maxParticipants ? 'error' : ''}
                        min="1"
                        onChange={(e) => setevent({...event, maxParticipants: e.target.value ? Number(e.target.value) : 0})}
                    />
                    {errors.maxParticipants && <span className="error-message">{errors.maxParticipants}</span>}
                    
                    <label htmlFor="imageData">Изображение</label>
                    <input
                        type={"file"}
                        id="imageData"
                        accept=".jpg,.jpeg,.png"
                        className={errors.imageData ? 'error' : ''}
                        onChange={(e) => setevent({...event, imageData: e.target.files?.[0] as File})}
                    />
                    {errors.imageData && <span className="error-message">{errors.imageData}</span>}
                    

                    

                </div>
                <div className="create-controll">
                    <button type="button" onClick={handleCancel}>Отмена</button>
                    <button type={"submit"} disabled={isLoad}>Создать</button>
                </div>
            </form>

            
            
        </div>
    );
};
