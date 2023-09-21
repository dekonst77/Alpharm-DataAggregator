(function () {

    angular.module('DataAggregatorModule').config([
        '$translateProvider', function ($translateProvider) {
            $translateProvider
                .translations('ru', getTranslations())
                .preferredLanguage('ru')
                .useSanitizeValueStrategy('escapeParameters');
        }
    ]);

    function getTranslations() {
        return {
            "COMMON": {
                "NAME": 'Наименование',
                "SHORT_NAME": 'Сокращённое наименование',
                "USER": 'Пользователь',
                "MANAGER": 'Руководитель'
            },

            "COMMON_GRID": {
                "SOURCE_NAME": 'Источник данных',
                "FILE_INFO_ID": 'Id файла',
                "FILE_PATH": 'Путь к файлу',
                "SELLING_PRICE": 'Цена продаж в руб',
                "SELLING_SUM": 'Сумма продаж в руб',
                "SELLING_COUNT": 'Сумма продаж в уп',
                "PURCHASE_PRICE": 'Цена закупок в руб',
                "PURCHASE_SUM": 'Сумма закупок в руб',
                "PURCHASE_COUNT": 'Сумма закупок в уп',
                "TYPE_FL": 'Тип ФЗ',
                "PURCHASE_NUMBER": 'Номер закупки',
                "PURCHASE_NAME": 'Наименование объекта закупки',
                "PURCHASE_CLASS": 'Раздел',
                "NAME": 'Наименование',
                "FIO_CLASS": 'ФИО ("Раздел")',
                "DATE_CLASS": 'Дата ("Раздел")',
                "REESTR_NUMBER": 'Реестровый номер',
                "CONTRACT_NUMBER": 'Номер контракта',
                "CONTRACT_STATUS": 'Статус контракта',
                "RECEIVER": 'Получатель',
                "SUPPLIER": 'Поставщик',
                "EDIT_DATE": "Дата изменения",
            },

            "CREATE_CONTRACT": {
                "URL": 'Ссылка',
                "SUM": 'Сумма',
                "ACTUALLY_PAID": 'Фактически оплачено',
                "CONTRACT_CONCLUSIONDATE": 'Дата заключения контракта',
                "CONTRACT_BEGINDATE": 'Дата начала исполнения контракта',
                "CONTRACT_ENDDATE": 'Дата окончания исполнения контракта',
            },

            "OFD": {
                "PRICE_ETALON": {
                    "TITLE": 'Эталонные цены',
                    "TITLE_v2": 'Эталонные цены v2',
                    "TITLE_new": 'Эталонные цены new'
                },
                "PRICE_HISTORY": {
                    "TITLE": 'Эталонные цены - периоды'
                },
                "DAILY_RELEASE": {
                    "MAIN": 'Ежедневный выпуск ОФД',
                    "PERIODS": 'Периоды дней'
                },
            },

            "PURCHASE_FOUND":
            {
                "CUSTOMER": 'Заказчик',
                "SEARCH_DATE": 'Дата поиска',
                "METHOD": 'Способ определения поставщика',
                "STAGE": 'Этап',
                "SUM": 'Сумма',
                "PUBLISH_DATE": 'Дата размещения закупки',
                "UPDATE_DATE": 'Обновлено',
                "PURCHASE_STAGE": 'Стадия процесса',
                "ERROR_MESSAGE": 'Ошибка'
            },

            "MAIN": {
                "TITLE": 'MAIN',
                "ADMINISTRATION": 'Администрирование',
                "CHANGE_PASSWORD": 'Сменить пароль',
                "ACCOUNT": {
                    "TITLE": 'Аккаунт'
                },
                "MANAGE_USER": {
                    "TITLE": 'Управление пользователями'
                },
                "DEPARTMENT_DICTIONARY": {
                    "TITLE": 'Справочник подразделений'
                },
                "NOTIFICATION": {
                    "TITLE": 'Уведомления'
                }
            },

            "PERIOD": 'Период',
            "PERIOD_START": 'Период с',
            "PERIOD_END": 'Период по',

            "REGION": 'Регион',
            "STATUS": 'Статус',
            "TRADE_NAME": 'Торговое наименование',
            "BRAND": 'Бренд',
            "DOP": 'DOP',
            "GOP": 'GOP',
            "DETAILING": 'Детализация',
            "OWNER_TRADE_MARK": 'Производитель',
            "PACKER": 'Упаковщик',
            "INN_GROUP": "МНН Групп",
            "GENERIC": "Дженерик",

            "BUTTONS":
            {
                "ADD": 'Добавить',
                "ADD_KEYWORD": 'Добавить ключевое слово',
                "ADD_PARAMETER": 'Добавить свойство',
                "APPLY": 'Применить',
                "CANCEL": 'Отменить',
                "CANCELLATION": 'Отмена',
                "CHANGE": 'Изменить',
                "CHANGE_ORGANIZATION_TYPE": 'Изменить тип организации',
                "CHANGE_REGION": 'Изменить регион',
                "CHANGE_REGION_OF_LOCALIZATION": 'Изменить регион локализации',
                "CHANGE_SECTION": 'Изменить раздел',
                "CHECK": 'Проверить',
                "CLEAR": 'Очистить',
                "CLEAR_FIELDS": 'Очистить поля',
                "CLEAR_PRICES": 'Очистить цены',
                "CLOSE": 'Закрыть',
                "COPY": 'Копировать',
                "CREATE": 'Создать',
                "DELETE": 'Удалить',
                "DELETE_KEYWORD": 'Удалить ключевое слово',
                "DISCARD": 'Снять',
                "DUPLICATE_PRICES": 'Дублировать цены',
                "EDIT": 'Редактировать',
                "EXPORT_TO_CSV": 'Экспортировать в CSV',
                "EXPORT_TO_EXCEL": 'Экспорт в Excel',
                "EXPORT_ALL_TO_EXCEL": 'Экспорт всё в Excel',
                "FILTER": 'Фильтр',
                "GET_DATA": 'Забрать',
                "HELP": 'Помощь',
                "LOAD_DATA": 'Загрузить данные',
                "MERGE": 'Объединить',
                "NO": 'Нет',
                "OK": 'OK',
                "RECALCULATE": 'Пересчитать',
                "RETURN_DATA": 'Вернуть',
                "RUN": 'Запустить',
                "CHANGE_GOODS_CATEGORY": 'Сменить категорию',
                "SAVE": 'Сохранить',
                "SEARCH": 'Искать',
                "SELECT": 'Выбрать',
                "STOP": 'Остановить',
                "UPDATE": 'Обновить',
                "TRANSFER": 'Перенести',
                "YES": 'Да',
                "HISTORY": 'История',
                "IMPORT": 'Импорт',
                "SET_PLUG_ON": 'Поставить заглушку',
                "SET_PLUG_OFF": 'Снять заглушку'
            },

            "ERROR":
            {
                "REQUIRED_FIELD": 'Поле обязательно для ввода',
                "IDS_FIELD": 'Поле должно быть перечислением целых чисел через запятую',
                "PERIOD": '\'Период с\' больше \'Период по\''
            },

            "YES": 'Да',
            "NO": 'Нет',

            "COUNTRIES":
            {
                "RUSSIA": 'Россия'
            },

            "CITIES": {
                "MOSCOW": 'Москва',
                "SAINT_PETERSBURG": 'Санкт-Петербург'
            },

            "MULTI_SELECT_DIALOG": {
                "SEMICOLON_SEPARATOR": 'Разделитель ;'
            },

            "AUTH_CHECKER": {
                "AUTO_LOGOUT_MESSAGE": 'Произошёл вход на другом устройстве.'
            },

            "SYSTEMATIZATION": {
                "SYSTEMATIZATION": {
                    "TITLE": 'Обработка данных'
                },
                "PERIODS_SETTINGS": {
                    "TITLE": 'Настройки'
                },
                "GOODS_SYSTEMATIZATION": {
                    "TITLE": 'Обработка доп. ассортимента'
                },
                "STATUS": {
                    "NAME": 'Статус',
                    'CHECKING': 'На проверку',
                    'ADDING': 'На заведение',
                    'OTHER': 'ДОП',
                    'ERROR': 'Ошибка',
                    'BOUND': 'Привязана',
                    'UNBOUND': 'Непривязана'
                }
            },

            "CLASSIFIER": {
                "TITLE": 'Классификация ЛС',
                "CLASSIFIER_EDITOR": {
                    "TITLE": 'Редактор классификатора'
                },
                "CLASSIFIER_EDITOR_CHANGE": {
                    "TITLE": 'Редактор классификатора - изменение'
                },
                "Manufacturer_EDITOR_CHANGE": {
                    "TITLE": 'Редактор производителей - изменение'
                },
                "GOODS_CLASSIFIER_EDITOR": {
                    "TITLE": 'Редактор доп. ассортимента'
                },
                "GOODS_CLASSIFIER_EDITOR_CHANGE": {
                    "TITLE": 'Редактор доп. ассортимента - изменение'
                },
                "GOODS_CATEGORY_EDITOR": {
                    "TITLE": 'Редактор категорий доп. ассортимента'
                },
                "GOODS_PARAMETERS_EDITOR": {
                    "TITLE": 'Редактор свойств доп. ассортимента'
                },
                "GOODS_CLASSIFIER_REPORT": {
                    "TITLE": 'Отчет по классификатору доп. ассортимента'
                },
                "SQA": {
                    "TITLE": "ПКУ"
                },
                "Orphan": {
                    "TITLE": "Орфанные"
                },
                "MASK": {
                    "TITLE": "Маска"
                },
                "FTG": {
                    "TITLE": 'Редактор ФТГ'
                },
                "ATC_WHO": {
                    "TITLE": 'Редактор ATCWho'
                },
                "ATC_EPHMRA": {
                    "TITLE": 'Редактор ATCEphmra'
                },
                "ATC_BAA": {
                    "TITLE": 'Редактор ATC Бад'
                },
                "NFC": {
                    "TITLE": 'Редактор NFC'
                },
                "VED": {
                    "TITLE": 'ЖНВЛП'
                },
                "FEDERAL_BENEFIT": {
                    "TITLE": 'Федеральная льгота'
                },
                "DATA_TRANSFER": {
                    "TITLE": 'Переброс данных'
                },
                "REPORT_BIONICA_MEDIA": {
                    "TITLE": 'Бионика Медиа'
                },
                "CLASSIFIER_RELEASE": {
                    "TITLE": 'Выпуск классификации данных'
                },
                "GENERIC": {
                    "TITLE": 'Дженерик'
                },
                "BLISTERBLOCK": {
                    "TITLE": 'Блок «блистеровка»'
                },
                "CHECK_REPORT": {
                    "TITLE": 'Проверка классификатора'
                },
                "AddingDOPtotheMonitoringDatabase": {
                    "TITLE": 'Добавление ДОП в БД мониторинг'
                },
                "ClassifierRxOtc": {
                    "TITLE": 'Модуль для простановки RX OTC'
                }
            },

            "GOVERNMENT_PURCHASES": {
                "TITLE": 'Госзакупки',
                "PURCHASE_LINK": {
                    "TITLE": 'Добавление закупок на скачивание'
                },
                "DISTRIBUTION_KEY_WORDS": {
                    "TITLE": 'Ключевые слова для распределения'
                },
                "DISTRIBUTION_WORK": {
                    "TITLE": 'Распределение в работу'
                },
                "CONTRACT_DISTRIBUTION_WORK": {
                    "TITLE": 'Распределение контрактов в работу'
                },
                "GOVERNMENT_PURCHASES": {
                    "TITLE": 'Редактирование закупок'
                },
                "ORGANIZATIONS_EDITOR": {
                    "TITLE": 'Редактирование организаций'
                },
                "SUPPLIERS": {
                    "TITLE": 'Редактирование поставщиков'
                },
                "MASS_FIXES_DATA": {
                    "TITLE": 'Блок массовой замены'
                },
                "OBJECTS_TO_OBJECTS_READY": {
                    "TITLE": 'Перенос данных по форме выпуска'
                },
                "CALC_RUNNER": {
                    "TITLE": 'Запуск расчета'
                },
                "CALCULATED_DATA_EDITOR": {
                    "TITLE": 'Редактирование расчетных данных'
                },
                "REPORT_DRUG_ID_WITH_MIN_MAX_PRICE": {
                    "TITLE": 'Drug id с min/max ценой'
                },
                "REPORT_WRONG_PRICES": {
                    "TITLE": 'Неправильные цены'
                },
                "REPORT_NOT_EXPORTED_TO_EXTERNAL_PURCHASES": {
                    "TITLE": 'Не вылитые в External базу закупки'
                },
                "REPORT_EXECUTION_TERMINATED_CONTRACT": {
                    "TITLE": 'Исполнение прекращено'
                },
                "REPORT_EXECUTION_CONTRACT": {
                    "TITLE": 'Исполнение контрактов'
                },
                "STATISTICS_PURCHASES_AND_CONTRACTS": {
                    "TITLE": 'Статистика по закупкам/контрактам'
                },
                "PURCHASES_FOUND": {
                    "TITLE": 'Найденные закупки'
                },
                "CLIENT_KEYWORDS": {
                    "TITLE": 'Редактор ключевых слов для клиентских фильтров'
                }
            },

            "RETAIL": {
                "TITLE": 'Розница',
                "RETAIL_TEMPLATES_TITLE": 'Редактор источников и шаблонов',
                "FILE_INFO_TITLE": 'Загрузка файлов',
                "SOURCE_PHARMACIES_EDITOR_TITLE": 'Справочник аптек',
                "PHARMACY_BRAND_BLACK_LIST_TITLE": 'Черный список аптека-бренд',
                "PHARMACY_WITHOUT_AVERAGE_LIST_TITLE": 'Аптеки, не идущие в расчёт по среднему ассортименту',
                "PHARMACY_OFD_BLACK_LIST_TITLE": 'Черный список для ОФД',
                "SOURCE_BRAND_BLACK_LIST_TITLE": 'Черный список брендов',
                "PRICE_LIMITS_EDITOR_TITLE": 'Редактор цен',
                "COUNT_CHECK_TITLE": 'Редактор количеств',
                "PRICE_RULE_EDITOR_TITLE": 'Корректировка цен на итоговых данных',
                "MARKUP_DEFAULT_EDITOR_TITLE": 'Корректировка наценок по умолчанию',
                "GOODS_PRICE_RULE_EDITOR": {
                    "TITLE": 'Корректировка цен доп. ассортимента на итоговых данных'
                },
                "GOODS_COUNT_RULE_EDITOR": {
                    "TITLE": 'Корректировка количеств доп. ассортимента на итоговых данных',
                    "FULL_VOLUME_TITLE": 'Корректировка количеств доп. ассортимента на итоговых данных - 100%'
                },
                "COUNT_RULE_EDITOR": {
                    "TITLE": 'Корректировка количеств на итоговых данных',
                    "FULL_VOLUME_TITLE": 'Корректировка количеств на итоговых данных - 100%',

                    "RULE": 'Правило',
                    "CHANGED": 'Изменено',
                    "USER": 'Пользователь',
                    "DATE": 'Дата',
                    "SOURCE": 'Источник',
                    "DESTINATION": 'Назначение',
                    "SOURCE_DESTINATION": 'Источник / Назначение',
                    "SOURCE_DESCRIPTION": 'Лекарственное средство, с которого переносить объёмы',
                    "DESTINATION_DESCRIPTION": 'Лекарственное средство, на которое переносить объёмы',
                    "TOP_COUNT": 'Топ (кол-во)',
                    "TOP_COUNT_FROM": 'Топ с',
                    "TOP_COUNT_TO": 'Топ на',
                    "PURCHASE_PERCENT": 'Закупки, %',
                    "SELLING_PERCENT": 'Продажи, %',
                    "PURCHASE_SUM": 'Закупки Сумма',
                    "SELLING_SUM": 'Продажи Сумма',
                    "PURCHASE_COUNT": 'Закупки Кол-во',
                    "SELLING_COUNT": 'Продажи Кол-во',
                    "ACTUALITY_TYPE": 'Актуальность',
                    "ACTUALITY_TYPES": {
                        "ALL": 'Все',
                        "ACTUAL": 'Только актуальные',
                        "NOT_ACTUAL": 'Только неактуальные'
                    },
                    "USED": 'Применен',
                    "OUT_USED": 'Продажи',
                    "IN_USED": 'Закупки',
                    "PURCHASE": "Закупки",
                    "SELLING": "Продажи",
                    "CLASSIFIER": "ClassifierId"

                },

                "SEARCH_RAW_DATA_BY_DRUG_CLEAR": {
                    "TITLE": 'Поиск в исходных данных по тексту',
                    "ORIGINAL_DRUG_NAME": 'Исходное написание препарата',
                    "ORIGINAL_MANUFACTURER_NAME": 'Исходное написание производителя',
                    "DRUG_CLEAR_IDS": 'Коды строк',
                    "PHARMACY_NAMES": 'Наименования аптек'
                },

                "RELEASE": {
                    "TITLE": 'Выпуск ретейла'
                },

                "REPORT": {
                    "TITLE": 'Отчеты ретейла'
                },

                "RETAIL_CALCULATION": {
                    "TITLE": 'Выпуск ретейла',
                    "START_PERIOD": 'Начать выпуск',
                    "END_PERIOD": 'Завершить выпуск',
                    "START_ACTION": 'Запустить',
                    "STOP_ACTION": 'Остановить',
                    "CALC_HISTORY": 'Пересчёт истории',
                    "FIELD": {
                        "ID": 'ID',
                        "USER": 'Пользователь',
                        "PROCESS": 'Процесс',
                        "STATUS": 'Статус',
                        "START_TIME": 'Время начала',
                        "END_TIME": 'Время конца',
                        "COMMENT": 'Комментарий',
                        "CHECK": 'Выбрать'
                    },
                    "LOG_FIELD": {
                        "ID": 'ID',
                        "STEP": 'Шаг',
                        "YEAR": 'Год',
                        "MONTH": 'Месяц',
                        "DATE": 'Время запуска'
                    }
                },

                "RETAIL_CTM": {
                    "TITLE": 'CTM'
                },

                "RETAIL_SALESSKUBYSF": {
                    "TITLE": 'Sell out'
                },

                "RULES_COMMIT": {
                    "TITLE": 'Фиксация правил'
                },

                "SEARCH_RAW_DATA_BY_CLASSIFIER": {
                    "TITLE": 'Поиск в исходных данных по классификатору',
                    "TARGET_PHARMACY_ID": 'Id аптеки',
                    "CLASSIFICATION_NAME_OF_DETAILING": 'Классификация в рамках детализации',

                    "PERIOD_DETAILING": {
                        "NAME": 'Детализация по периоду',
                        'NONE': 'Без разбивки',
                        'YEAR': 'По годам',
                        'MONTH': 'По месяцам'
                    },

                    "GOODS_TITLE": 'Поиск в исходных данных по доп. ассортименту'
                }
            },
            "GS": {
                "TITLE": 'Ген.Совокупность',
                "Reestr": {
                    "TITLE": 'Реестр ГС'
                },
                "Licenses": {
                    "TITLE": 'Лицензии Росздравнадзор'
                },
                "BookOfChange": {
                    "FormingTransaction": {
                        "CodeAS1": "Код сети 1",
                        "Who": "Кто",
                        "CodeAS2": "Код сети 2",
                        "Whom": "Кого",
                        "Region": "Регион",
                        "WhenPeriod": "Когда",
                        "CountAS": "Сколько",
                        "Comment": "Комментарий",
                        "Show": "Показывать",
                    },
                    "Rebranding": {
                        "CodeAS1": "Код сети 1",
                        "PrevName": "Было",
                        "CodeAS2": "Код сети 2",
                        "CurrentName": "Стало",
                        "WhenPeriod": "С какого периода",
                        "Comment": "Комментарий",
                        "Show": "Показывать",
                    }
                }
            },
            "REPORTS": {
                "TITLE": 'Отчёты'
            },
            "STATISTICS": {
                "TITLE": 'Статистика'
            },

            "MESSAGE_BOX_SERVICE": {
                "CONFIRMATION": 'Подтверждение',
                "ERROR": 'Ошибка',
                "INFO": 'Информация'
            }
        };
    }
})();