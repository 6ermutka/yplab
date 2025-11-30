// ==UserScript==
// @name         Помощник для изучения экзаменационных вопросов Tests24
// @namespace    http://tampermonkey.net/
// @version      1.3
// @description  Помогает проверять ответы на экзаменационные вопросы для tests24.su
// @author       You
// @match        https://tests24.su/*
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    const questionsData = {
        "questions": [
            {
                "id": 1,
                "question": "Что, согласно Правилам устройства электроустановок, называется приемником электрической энергии (электроприемником)?",
                "answer": "Аппарат, агрегат и др., предназначенный для преобразования электрической энергии в другой вид энергии"
            },
            {
                "id": 2,
                "question": "Кто проводит первичный инструктаж командированному персоналу при проведении работ в электроустановках до 1000 В?",
                "answer": "Работник организации - владельца электроустановок из числа административно-технического персонала, имеющий группу IV"
            },
            {
                "id": 3,
                "question": "В каком из перечисленных случаев допускается выдавать один наряд-допуск для одновременного или поочередного выполнения работ на разных рабочих местах одной электроустановки?",
                "answer": "Во всех перечисленных"
            },
            {
                "id": 4,
                "question": "Какой тип опор устанавливается в местах изменения направления трассы воздушной линии электропередачи?",
                "answer": "Угловые опоры"
            },
            {
                "id": 5,
                "question": "Что представляет собой электропроводка согласно Правилам технической эксплуатации электроустановок потребителей электрической энергии, утвержденным приказом Министерства энергетики Российской Федерации от 12 августа 2022 N 811?",
                "answer": "Совокупность проводов и кабелей с относящимися к ним креплениями, установочными и защитными деталями, проложенных по поверхности или внутри конструктивных строительных элементов"
            },
            {
                "id": 6,
                "question": "Каким цветом должны быть обозначены рукоятки приводов заземляющих разъединителей (ножей), ведущие валы двигательных приводов заземляющих ножей и заземляющие ножи?",
                "answer": "Красным цветом - рукоятки приводов заземляющих разъединителей (ножей), а также ведущие валы двигательных приводов заземляющих ножей, а заземляющие ножи - черным цветом"
            },
            {
                "id": 7,
                "question": "Какую группу по электробезопасности должен иметь допускающий в электроустановках напряжением до 1000 В?",
                "answer": "Группу III по электробезопасности"
            },
            {
                "id": 8,
                "question": "Какую подготовку необходимо иметь работникам, принимаемым для выполнения работ в электроустановках?",
                "answer": "Профессиональную подготовку и квалификацию, соответствующую характеру работы и выполняемым должностным обязанностям (трудовым функциям)"
            },
            {
                "id": 9,
                "question": "На кого распространяются Правила по охране труда при эксплуатации электроустановок?",
                "answer": "На работодателей - юридических и физических лиц независимо от их организационно-правовых форм и работников из числа электротехнического, электротехнологического и не электротехнического персонала"
            },
            {
                "id": 10,
                "question": "Укажите перечень исчерпывающих мероприятий по оказанию первой помощи, в соответствии с приказом Минздрава России от 03.05.2024 № 220н.",
                "answer": "1) Проведение оценки обстановки и обеспечение безопасных условий для оказания первой помощи. 2) Проведение обзорного осмотра пострадавшего (пострадавших) для выявления продолжающегося наружного кровотечения. При необходимости осуществление мероприятий по временной остановке наружного кровотечения одним или несколькими способами. 3) Определение наличия признаков жизни у пострадавшего. 4) Проведение сердечно-легочной реанимации и поддержание проходимости дыхательных путей. 5) Проведение подробного осмотра и опроса пострадавшего (при наличии сознания) для выявления признаков травм, ранений, отравлений, укусов или ужаливаний ядовитых животных, поражений, вызванных механическими, химическими, электрическими, термическими поражающими факторами, воздействием излучения, и других состояний, угрожающих его жизни и здоровью: 6) Выполнение мероприятий по оказанию первой помощи пострадавшему в зависимости от характера травм, ранений, отравлений, укусов или ужаливании ядовитых животных, поражений, вызванных механическими, химическими, электрическими, термическими поражающими факторами, воздействием излучения, и других состояний, угрожающих его жизни и здоровью. 7) Оказание помощи пострадавшему в принятии лекарственных препаратов для медицинского применения, назначенных ему ранее лечащим врачом. 8) Придание и поддержание оптимального положения тела пострадавшего. 9) Вызов скорой медицинской помощи (если вызов скорой медицинской помощи не был осуществлен ранее), осуществление контроля состояния пострадавшего (наличия сознания, дыхания, кровообращения и отсутствия наружного кровотечения), оказание пострадавшему психологической поддержки, перемещение, транспортировка пострадавшего, передача пострадавшего выездной бригаде скорой медицинской помощи, медицинской организации, специальным службам, сотрудники которых обязаны оказывать первую помощь в соответствии с федеральными законами или иными нормативными правовыми актами"
            }
        ]
    };

    class ExamHelper {
        constructor() {
            this.questions = questionsData.questions;
            this.isEnabled = true;
            this.processedQuestions = new Set();
            this.init();
        }

        init() {
            console.log('🚀 ExamHelper инициализирован');
            console.log(`📚 Загружено вопросов: ${this.questions.length}`);
            this.createUI();
            this.startMonitoring();
        }

        createUI() {
            console.log('🛠️ Создание интерфейса...');

            this.uiContainer = document.createElement('div');
            this.uiContainer.style.cssText = `
                position: fixed;
                top: 10px;
                right: 10px;
                background: #fff;
                border: 2px solid #4CAF50;
                padding: 15px;
                border-radius: 8px;
                z-index: 10000;
                max-width: 350px;
                font-family: Arial, sans-serif;
                font-size: 14px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            `;

            this.answerDisplay = document.createElement('div');
            this.answerDisplay.innerHTML = '<strong style="color: #4CAF50;">✓ Помощник вопросов</strong><br>Найдите вопрос на странице';

            this.toggleBtn = document.createElement('button');
            this.toggleBtn.textContent = 'Выключить';
            this.toggleBtn.style.cssText = `
                margin-top: 10px;
                padding: 5px 10px;
                background: #4CAF50;
                color: white;
                border: none;
                border-radius: 4px;
                cursor: pointer;
            `;
            this.toggleBtn.onclick = () => this.toggleHelper();

            this.statsDisplay = document.createElement('div');
            this.statsDisplay.style.cssText = `
                margin-top: 10px;
                font-size: 12px;
                color: #666;
            `;

            this.uiContainer.appendChild(this.answerDisplay);
            this.uiContainer.appendChild(this.toggleBtn);
            this.uiContainer.appendChild(this.statsDisplay);
            document.body.appendChild(this.uiContainer);

            console.log('✅ Интерфейс создан');
        }

        toggleHelper() {
            this.isEnabled = !this.isEnabled;
            this.toggleBtn.textContent = this.isEnabled ? 'Выключить' : 'Включить';
            this.toggleBtn.style.background = this.isEnabled ? '#4CAF50' : '#f44336';
            this.answerDisplay.innerHTML = this.isEnabled ?
                '<strong style="color: #4CAF50;">✓ Помощник включен</strong>' :
                '<strong style="color: #f44336;">✗ Помощник выключен</strong>';

            console.log(`🔧 Помощник ${this.isEnabled ? 'включен' : 'выключен'}`);

            if (this.isEnabled) {
                this.processQuestions();
            }
        }

        startMonitoring() {
            console.log('👀 Запуск мониторинга страницы...');

            // Сразу обрабатываем существующие вопросы
            setTimeout(() => {
                this.processQuestions();
            }, 1000);

            // Наблюдатель за изменениями
            const observer = new MutationObserver((mutations) => {
                if (this.isEnabled) {
                    let shouldProcess = false;
                    mutations.forEach(mutation => {
                        if (mutation.addedNodes.length > 0) {
                            shouldProcess = true;
                        }
                    });

                    if (shouldProcess) {
                        console.log('🔄 Обнаружены изменения DOM, обработка вопросов...');
                        setTimeout(() => this.processQuestions(), 500);
                    }
                }
            });

            observer.observe(document.body, {
                childList: true,
                subtree: true
            });

            console.log('✅ Мониторинг запущен');
        }

        processQuestions() {
            console.log('🔍 Поиск вопросов на странице...');
            const questionContainers = document.querySelectorAll('.watu-question');

            console.log(`📋 Найдено контейнеров вопросов: ${questionContainers.length}`);

            let processedCount = 0;
            let matchedCount = 0;

            questionContainers.forEach((container, index) => {
                const containerId = container.id || `container-${index}`;

                if (this.processedQuestions.has(containerId)) {
                    console.log(`⏩ Пропуск уже обработанного контейнера: ${containerId}`);
                    return;
                }

                console.log(`🔎 Обработка контейнера: ${containerId}`);
                processedCount++;

                const questionText = this.extractQuestionText(container);
                if (questionText) {
                    console.log(`📖 Извлечен текст вопроса: "${questionText.substring(0, 50)}..."`);
                    const result = this.processQuestion(container, questionText, containerId);
                    if (result) matchedCount++;
                } else {
                    console.log(`❌ Не удалось извлечь текст вопроса из ${containerId}`);
                }

                this.processedQuestions.add(containerId);
            });

            this.updateStats(processedCount, matchedCount);
            console.log(`📊 Итоги: обработано ${processedCount}, найдено совпадений ${matchedCount}`);
        }

        extractQuestionText(container) {
            const questionContent = container.querySelector('.question-content');
            if (!questionContent) {
                console.log('❌ Не найден .question-content');
                return null;
            }

            const questionElement = questionContent.querySelector('strong');
            if (questionElement) {
                const text = questionElement.textContent.trim();
                console.log(`✅ Текст вопроса из strong: "${text.substring(0, 30)}..."`);
                return text;
            }

            // Альтернативный метод извлечения
            const text = questionContent.textContent.replace(/^\d+\.\s*/, '').trim();
            console.log(`✅ Текст вопроса из общего контента: "${text.substring(0, 30)}..."`);
            return text;
        }

        processQuestion(container, questionText, containerId) {
            console.log(`🔍 Поиск совпадения для вопроса: "${questionText.substring(0, 40)}..."`);

            const matchedQuestion = this.findMatchingQuestion(questionText);

            if (matchedQuestion) {
                console.log(`✅ Найдено совпадение! ID вопроса: ${matchedQuestion.id}`);
                console.log(`📝 Ответ: ${matchedQuestion.answer.substring(0, 50)}...`);

                if (this.isEnabled) {
                    this.addAnswerToContainer(container, matchedQuestion);
                    console.log(`✅ Ответ добавлен в контейнер ${containerId}`);
                } else {
                    console.log(`⏸️ Помощник выключен, ответ не добавлен`);
                }

                return true;
            } else {
                console.log(`❌ Совпадение не найдено для контейнера ${containerId}`);
                return false;
            }
        }

        findMatchingQuestion(text) {
            const cleanText = text.trim().toLowerCase();
            console.log(`🔍 Поиск совпадения для: "${cleanText.substring(0, 50)}..."`);

            // Точное совпадение
            let matched = this.questions.find(q => {
                const qText = q.question.toLowerCase().trim();
                return qText === cleanText;
            });

            if (matched) {
                console.log(`🎯 Найдено точное совпадение: ID ${matched.id}`);
                return matched;
            }

            // Частичное совпадение (первые 20 символов)
            matched = this.questions.find(q => {
                const qText = q.question.toLowerCase().trim();
                const compareLength = Math.min(20, qText.length, cleanText.length);
                return qText.substring(0, compareLength) === cleanText.substring(0, compareLength);
            });

            if (matched) {
                console.log(`🔍 Найдено частичное совпадение: ID ${matched.id}`);
                return matched;
            }

            // Поиск по ключевым словам
            matched = this.questions.find(q => {
                const qText = q.question.toLowerCase();
                const keywords = cleanText.split(' ').filter(word => word.length > 4);
                return keywords.some(keyword => qText.includes(keyword));
            });

            if (matched) {
                console.log(`🔑 Найдено совпадение по ключевым словам: ID ${matched.id}`);
                return matched;
            }

            console.log(`❌ Совпадений не найдено`);
            return null;
        }

        addAnswerToContainer(container, questionData) {
            // Удаляем старый блок ответа если есть
            const oldAnswer = container.querySelector('.exam-helper-answer');
            if (oldAnswer) {
                oldAnswer.remove();
                console.log('🗑️ Удален старый блок ответа');
            }

            const answerBlock = document.createElement('div');
            answerBlock.className = 'exam-helper-answer';
            answerBlock.style.cssText = `
                background: #e8f5e8;
                border: 2px solid #4CAF50;
                border-radius: 6px;
                padding: 12px;
                margin-bottom: 15px;
                font-family: Arial, sans-serif;
                font-size: 14px;
                color: #2e7d32;
            `;

            answerBlock.innerHTML = `
                <div style="font-weight: bold; margin-bottom: 8px; color: #1b5e20;">
                    ✓ Правильный ответ (вопрос ${questionData.id}):
                </div>
                <div style="font-style: italic;">${questionData.answer}</div>
            `;

            // Безопасная вставка - в начало контейнера
            try {
                if (container.firstChild) {
                    container.insertBefore(answerBlock, container.firstChild);
                    console.log(`✅ Блок ответа успешно добавлен в начало контейнера`);
                } else {
                    container.appendChild(answerBlock);
                    console.log(`✅ Блок ответа успешно добавлен в контейнер`);
                }
            } catch (error) {
                console.error(`❌ Ошибка при вставке блока ответа:`, error);
                // Альтернативный метод - добавляем в body рядом с контейнером
                this.addAnswerNearContainer(container, questionData);
            }
        }

        addAnswerNearContainer(container, questionData) {
            console.log('🔄 Использование альтернативного метода вставки...');

            const answerBlock = document.createElement('div');
            answerBlock.className = 'exam-helper-answer-alternative';
            answerBlock.style.cssText = `
                background: #e8f5e8;
                border: 2px solid #4CAF50;
                border-radius: 6px;
                padding: 12px;
                margin: 10px 0;
                font-family: Arial, sans-serif;
                font-size: 14px;
                color: #2e7d32;
                position: relative;
                z-index: 1000;
            `;

            answerBlock.innerHTML = `
                <div style="font-weight: bold; margin-bottom: 8px; color: #1b5e20;">
                    ✓ Правильный ответ (вопрос ${questionData.id}):
                </div>
                <div style="font-style: italic;">${questionData.answer}</div>
            `;

            // Вставляем перед контейнером вопроса
            if (container.parentNode) {
                container.parentNode.insertBefore(answerBlock, container);
                console.log(`✅ Блок ответа добавлен перед контейнером вопроса`);
            } else {
                document.body.insertBefore(answerBlock, document.body.firstChild);
                console.log(`✅ Блок ответа добавлен в начало body`);
            }
        }

        updateStats(processed, matched) {
            this.statsDisplay.textContent = `Обработано: ${processed}, Найдено: ${matched}`;
        }
    }

    // Запуск скрипта
    console.log('🎬 Загрузка скрипта ExamHelper...');

    if (document.readyState === 'loading') {
        console.log('⏳ Документ загружается, ожидание DOMContentLoaded...');
        document.addEventListener('DOMContentLoaded', () => {
            console.log('✅ DOM загружен, запуск ExamHelper');
            new ExamHelper();
        });
    } else {
        console.log('✅ DOM уже загружен, немедленный запуск ExamHelper');
        new ExamHelper();
    }
})();
