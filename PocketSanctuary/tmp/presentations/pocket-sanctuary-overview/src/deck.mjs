const {
  Presentation,
  PresentationFile,
  column,
  text,
  shape,
  fill,
  fixed,
  wrap,
  hug,
  layers,
} = await import("@oai/artifact-tool");

const SLIDE_WIDTH = 1920;
const SLIDE_HEIGHT = 1080;

const palette = {
  bg: "#F7FAFF",
  panel: "#E6F5EC",
  title: "#143D2A",
  body: "#244B3A",
  accent: "#7E60BF",
};

const presentation = Presentation.create({
  slideSize: { width: SLIDE_WIDTH, height: SLIDE_HEIGHT },
});

function addSlide(title, body) {
  const slide = presentation.slides.add();
  slide.compose(
    layers({ width: fill, height: fill }, [
      shape({
        kind: "rect",
        width: fill,
        height: fill,
        fill: palette.bg,
      }),
      column(
        {
          width: fill,
          height: fill,
          padding: { x: 120, y: 90 },
          gap: 28,
        },
        [
          text(title, {
            width: fill,
            height: hug,
            style: {
              fontSize: 64,
              bold: true,
              color: palette.title,
            },
          }),
          shape({
            kind: "rect",
            width: fixed(260),
            height: fixed(8),
            fill: palette.accent,
          }),
          text(body, {
            width: wrap(1600),
            height: hug,
            style: {
              fontSize: 34,
              color: palette.body,
              lineHeight: 1.3,
            },
          }),
        ],
      ),
    ]),
    {
      frame: { left: 0, top: 0, width: SLIDE_WIDTH, height: SLIDE_HEIGHT },
      baseUnit: 8,
    },
  );
}

addSlide(
  "Pocket Sanctuary",
  "A cozy mobile sanctuary game in Unity.\nPlayers collect exotic animals, care for them, customize their look and habitats, and progress through daily rewards.",
);

addSlide(
  "Overview",
  "• Category: Gaming / Simulation / Entertainment\n• Mobile-first touch gameplay for short check-in sessions\n• Relaxing progression centered on care, customization, and growth\n• Built for casual gamers and animal lovers",
);

addSlide(
  "Core Gameplay Loop",
  "• Log in and enter your sanctuary\n• Check your current pocketpal stats (hunger, mood, sleep, level)\n• Interact through care actions (feed, pet, maintain habitat)\n• Complete tasks and claim rewards\n• Use coins to unlock and customize content",
);

addSlide(
  "Required Features",
  "• Login flow before entering gameplay\n• Multiple exotic pocketpals to manage\n• Care interactions and pet progression\n• Multi-screen navigation (login, sanctuary, detail, shop/customization, settings/profile)\n• Persistent saved progress",
);

addSlide(
  "Optional Features",
  "• Push notifications for care reminders\n• Additional habitats/biomes as sanctuary grows\n• Daily rewards, streak systems, and limited-time events",
);

addSlide(
  "Screen Archetypes",
  "• Login: credentials and entry point\n• Sanctuary (Stream): main home gameplay view\n• Detail: selected animal stats and care actions\n• Creation: customization for pals and habitats\n• Profile/Settings: account progression and preferences",
);

addSlide(
  "Navigation",
  "Tabs/Screens: Login, Sanctuary, PocketPal, Shop, TasksRewards, Customization, Settings\n\nCore flow:\n• Login → Sanctuary\n• Sanctuary → Animal Detail / Shop / Customization\n• Tasks & Rewards → Claim progress rewards\n• Settings → adjust preferences or log out",
);

addSlide(
  "Milestone Progress (Draft)",
  "Milestone 1: product spec + wireframes\nMilestone 2: Unity project foundation, login, sanctuary base layout\nMilestone 3: iterative gameplay expansion (tasks, rewards, customization, progression)\n\nThis deck summarizes the current app direction from brainstorm.md.",
);

{
  const slide = presentation.slides.add();
  slide.compose(
    layers({ width: fill, height: fill }, [
      shape({
        kind: "rect",
        width: fill,
        height: fill,
        fill: palette.bg,
      }),
      column(
        {
          width: fill,
          height: fill,
          padding: { x: 110, y: 70 },
          gap: 24,
        },
        [
          text("Wireframe Reference", {
            width: fill,
            height: hug,
            style: {
              fontSize: 62,
              bold: true,
              color: palette.title,
            },
          }),
          shape({
            kind: "rect",
            width: fill,
            height: fixed(520),
            fill: palette.panel,
          }),
          text(
            "Wireframe source file:\nimages/NewWireframes.png\n\nUse this as the visual reference for login, sanctuary, pocketpal, customization, tasks & rewards, and settings flows.",
            {
              width: wrap(1500),
              height: hug,
              style: {
                fontSize: 36,
                color: palette.body,
                lineHeight: 1.3,
                align: "center",
              },
            },
          ),
        ],
      ),
    ]),
    {
      frame: { left: 0, top: 0, width: SLIDE_WIDTH, height: SLIDE_HEIGHT },
      baseUnit: 8,
    },
  );
}

const pptxBlob = await PresentationFile.exportPptx(presentation);
await pptxBlob.save("output/output.pptx");
