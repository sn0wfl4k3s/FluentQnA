# FluentQnA
Uma lib de machine learning para simular um serviço cognitivo para responder perguntas de usuários consultando uma base de conhecimento previamente carregada. A base de conhecimento pode ser em xlsx ou json. Essa lib pode ser usada para facilitar a criação de chatbots consultando pergunta e respostas roteirizadas e usando classificação multiclasse para selecionar as melhores respostas com base na precisão.

## Modo de uso para web api:
Primeiro adicione a base de conhecimento:  
![project](https://user-images.githubusercontent.com/30809620/70176657-9b62ef80-16b7-11ea-9e7f-e3709e120c5d.PNG)  

  Você pode baixar a base de conhecimento de exemplo [aqui](https://github.com/snowflakesss/FluentQnA/blob/master/FluentQnA/WebApplication/knowledgebase.xlsx).
  
Não se esqueça de setar as Properties da knowledgebase.xlsx para Copy if newer:
![properties](https://user-images.githubusercontent.com/30809620/70178791-a455c000-16bb-11ea-95b2-3e17ba3946c7.PNG)

Depois adicione na Startup.cs:  
![services](https://user-images.githubusercontent.com/30809620/70176814-e54bd580-16b7-11ea-9f11-d116c5275637.PNG)  


Agora é só utilizar no controller da seguinte forma:  
![controller](https://user-images.githubusercontent.com/30809620/70176922-17f5ce00-16b8-11ea-8c79-be5369a522d5.PNG)
